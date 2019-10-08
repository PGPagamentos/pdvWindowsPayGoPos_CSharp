using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace POSExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool isRunning = false;

        private int appListeningPort = 10000;
        private int currentNumberOfTerminals = 0;
        private int maxNumberOfTerminals = 50;

        private string msgIdle = "";
        private string appCompany = "NTK Solutions";
        private string appVersion = "Aplicacao exemplo 1.0";
        private string appWorkingPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\PayGoPOS";

        Dictionary<int, string> PTIRET_MSG = new Dictionary<int, string>()
        {
            {(int)ClassPOSPGW.PTIRET.ERRO_INTERNO, "Erro desta aplicação"},
            {(int)ClassPOSPGW.PTIRET.PTIRET_INVPARAM, "Parametro invalido"},
            {(int)ClassPOSPGW.PTIRET.PTIRET_NOCONN, "Terminal offline"},
            {(int)ClassPOSPGW.PTIRET.PTIRET_BUSY, "Terminal ocupado \r processando outro comando"},
            {(int)ClassPOSPGW.PTIRET.PTIRET_TIMEOUT, "Timeout"},
            {(int)ClassPOSPGW.PTIRET.PTIRET_CANCEL, "Tecla[CANCEL] pressionada"},
            {(int)ClassPOSPGW.PTIRET.PTIRET_NODATA, "Informação nao avaliavel"},
            {(int)ClassPOSPGW.PTIRET.PTIRET_BUFOVRFLW, "Dados maiores \r que o buffer"},
            {(int)ClassPOSPGW.PTIRET.PTIRET_SOCKETERR, "Erro de Socket"},
            {(int)ClassPOSPGW.PTIRET.PTIRET_WRITEERR, "Nao consegue acessar \r o diretorio"},
            {(int)ClassPOSPGW.PTIRET.PTIRET_EFTERR, "Operacao EFT foi completada,\r mas  falhou"},
            {(int)ClassPOSPGW.PTIRET.PTIRET_INTERNALERR, "Erro interno da DLL"},
            {(int)ClassPOSPGW.PTIRET.PTIRET_PROTOCOLERR, "Erro de comunicacao entre \r a DLL e o  terminal"},
            {(int)ClassPOSPGW.PTIRET.PTIRET_SECURITYERR, "Funcao falhou devido \r a problemas de seguranca"},
            {(int)ClassPOSPGW.PTIRET.PTIRET_PRINTERR, "Erro  de Impressora"},
            {(int)ClassPOSPGW.PTIRET.PTIRET_NOPAPER, "Terminou o papel da \r impressora"},
            {(int)ClassPOSPGW.PTIRET.PTIRET_NEWCONN, "Novo terminal conectado"},
            {(int)ClassPOSPGW.PTIRET.PTIRET_NONEWCONN, "Sem recebimento de \r novas conexões"},
            {(int)ClassPOSPGW.PTIRET.PTIRET_NOTSUPPORTED, "Funcao nao suportada \r pelo terminal"},
            {(int)ClassPOSPGW.PTIRET.PTIRET_CRYPTERR, "Erro na encriptacao \r de dados "},


        };
        Thread t;

        public MainWindow()
        {
            InitializeComponent();

        }

        private void WaitForTerminalConnection()
        {
            while (isRunning)
            {
                try
                {
                    short ret = 99;
                    StringBuilder terminalId = new StringBuilder(30);
                    StringBuilder mac = new StringBuilder(30);
                    StringBuilder model = new StringBuilder(30);
                    StringBuilder serialNumber = new StringBuilder(50);

                    //Verifica se algum terminal conectou:
                    ClassPOSPGW.PTI_ConnectionLoop(terminalId, model, mac, serialNumber, ref ret);

                    if (ret == (short)ClassPOSPGW.PTIRET.PTIRET_NEWCONN)
                    {
                        ClassTerminalInformation termInfo = new ClassTerminalInformation();

                        termInfo.terminalId = terminalId.ToString();
                        termInfo.model = model.ToString();
                        termInfo.mac = mac.ToString();
                        termInfo.serialNumber = serialNumber.ToString();

                        Thread t = new Thread(TerminalResponseThread);
                        t.Start(termInfo);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    Thread.Sleep(300);
                }
            }
        }

        private void TerminalResponseThread(object receivedObject)
        {
            ClassTerminalInformation termInfo = (ClassTerminalInformation)receivedObject;
            string terminalId = termInfo.terminalId;

            Debug.Print("Inicio da Thread (" + Thread.CurrentThread.ManagedThreadId + " - " + terminalId + "): " + DateTime.Now.ToShortTimeString());
            currentNumberOfTerminals = currentNumberOfTerminals + 1;

            short key = 99;
            short ret = 99;
            
            System.Text.StringBuilder amount = new System.Text.StringBuilder(30);
            System.Text.StringBuilder mac = new System.Text.StringBuilder(30);
            System.Text.StringBuilder model = new System.Text.StringBuilder(30);
            System.Text.StringBuilder serialnumber = new System.Text.StringBuilder(50);

            //Mostra ao usuário o identificador do terminal que conectou:
            ClassPOSPGW.PTI_Display(terminalId, "TERMINAL\r" + terminalId + "\rCONECTADO", ref ret);

            //Usa função de aguardar tecla para deixar mensagem anterior na tela por 5 segundos:
            ClassPOSPGW.PTI_WaitKey(terminalId, 5, ref key, ref ret);

            //Consulta informações do terminal através da função PTI_CheckStatus:
            //ClassPOSPGW.PTI_CheckStatus(terminalId, ref status, model, mac, serialnumber, ref ret);

            //Mostra ao usuário os dados obtidos através da função PTI_CheckStatus:
            //ClassPOSPGW.PTI_Display(terminalId, "SERIAL: " + serialnumber + "\rMAC: " + mac + "\rMODELO: " + model, ref ret);


            bool retorno = false;

            retorno = MenuPrincipal(terminalId);




            //Usa função de aguardar tecla para deixar mensagem anterior na tela por 5 segundos:
            ClassPOSPGW.PTI_WaitKey(terminalId, 5, ref key, ref ret);


            ClassPOSPGW.PTI_Disconnect(terminalId, 0, ref ret);

            currentNumberOfTerminals = currentNumberOfTerminals - 1;

            Debug.Print("Final da Thread (" + Thread.CurrentThread.ManagedThreadId + " - " + terminalId + "): " + DateTime.Now.ToShortTimeString());
            
        }

        private void btnIniciar_Click(object sender, RoutedEventArgs e)
        {
            currentNumberOfTerminals = 0;

            if (!System.IO.Directory.Exists(appWorkingPath)) System.IO.Directory.CreateDirectory(appWorkingPath);

            short ret = 99;
            ClassPOSPGW.PTI_Init(appCompany, appVersion, "24", appWorkingPath, appListeningPort, maxNumberOfTerminals, msgIdle, 0, ref ret);

            if (ret == 0)
            {
                isRunning = true;
                btnIniciar.IsEnabled = false;
                btnParar.IsEnabled = true;

                t = new Thread(WaitForTerminalConnection);
                t.Start();

            }
            else
            {
                MessageBox.Show("ERRO AO INICIAR DLL: \r" + (ClassPOSPGW.PTIRET)ret, "ERRO");
            }
        }

        private void btnParar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                t.Abort();
            }
            catch (Exception) { }


            isRunning = false;

            ClassPOSPGW.PTI_End();

            btnParar.IsEnabled = false;
            btnIniciar.IsEnabled = true;
            currentNumberOfTerminals = 0;
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            btnParar_Click(null, null);

            Application.Current.Shutdown();
        }

        public bool MenuPrincipal(string terminalId)
        {
                      
            short ret = 99;
            short key = 99;
            short selectedOption = -1;
            System.Text.StringBuilder amount = new System.Text.StringBuilder(30);


            while (true)
            {
                //Inicia função de menu:
                ClassPOSPGW.PTI_StartMenu(terminalId, ref ret);
                //Adiciona opção 1 do menu:
                ClassPOSPGW.PTI_AddMenuOption(terminalId, "Operacoes", ref ret);
                //Adiciona opção 2 ao menu:
                ClassPOSPGW.PTI_AddMenuOption(terminalId, "Captura de Dados", ref ret);
                //Adiciona opção 3 ao menu:
                ClassPOSPGW.PTI_AddMenuOption(terminalId, "Impressao", ref ret);


                //Executa o menu:
                ClassPOSPGW.PTI_ExecMenu(terminalId, "SELECIONE A OPCAO", 10, ref selectedOption, ref ret);

                //////////


                if (ret == (int)ClassPOSPGW.PTIRET.PTIRET_TIMEOUT)
                {
                    ClassPOSPGW.PTI_Display(terminalId, "Tempo de Espera \r  Esgotado(TIMEOUT)", ref ret);
                    // Usa função de aguardar tecla para deixar mensagem anterior na tela por 5 segundos:
                    ClassPOSPGW.PTI_WaitKey(terminalId, 5, ref key, ref ret);
                    return false;

                }
                //////
                else
                {
                    //Mostra para o usuário a opção selecionada por ele:
                    //ClassPOSPGW.PTI_Display(terminalId, "OPCAO SELECIONADA: " + selectedOption, ref ret);
                    if (selectedOption == 0) // menu de operacoes
                    {
                        MenuOperacoes(terminalId);

                    }
                    else if (selectedOption == 1)
                    {
                        MenuCaptura(terminalId);
                    }
                    else if (selectedOption == 2)
                    {
                        MenuImpressao(terminalId);
                    }
                }
            }
                
        }

        public bool MenuOperacoes(string terminalId)
        {

            short key = 99;
            short ret = 99;
            short selectedOption = -1;
            System.Text.StringBuilder amount = new System.Text.StringBuilder(30);

            while (true)
            {
                //Inicia função de menu:
                ClassPOSPGW.PTI_StartMenu(terminalId, ref ret);

                //Adiciona opção 1 ao menu:
                ClassPOSPGW.PTI_AddMenuOption(terminalId, "Venda", ref ret);
                //Adiciona opção 2 ao menu:
                ClassPOSPGW.PTI_AddMenuOption(terminalId, "Cancelamento", ref ret);
                //Adiciona opção 3 ao menu:
                ClassPOSPGW.PTI_AddMenuOption(terminalId, "Administrativa", ref ret);
                //Adiciona opção 4 ao menu:
                ClassPOSPGW.PTI_AddMenuOption(terminalId, "Retornar", ref ret);


                //Executa o menu:
                ClassPOSPGW.PTI_ExecMenu(terminalId, "SELECIONE A OPCAO", 30, ref selectedOption, ref ret);

                if (ret == (int)ClassPOSPGW.PTIRET.PTIRET_TIMEOUT)
                {
                    ClassPOSPGW.PTI_Display(terminalId, "Tempo de Espera \r  Esgotado(TIMEOUT)", ref ret);
                    // Usa função de aguardar tecla para deixar mensagem anterior na tela por 5 segundos:
                    ClassPOSPGW.PTI_WaitKey(terminalId, 5, ref key, ref ret);
                    return false;

                }
                else
                {
                    if (selectedOption == 0) // menu de venda
                    {
                        OperacaoVenda(terminalId);
                        
                    }
                    else if (selectedOption == 1) // cancelamento
                    {
                        Cancelamento(terminalId);
                        //Usa função de aguardar tecla para deixar mensagem anterior na tela por 5 segundos:
                        ClassPOSPGW.PTI_WaitKey(terminalId, 5, ref key, ref ret);

                    }
                    else if (selectedOption == 2) // Funcao Administrativa
                    {
                        //Usa função de aguardar tecla para deixar mensagem anterior na tela por 5 segundos:
                        ClassPOSPGW.PTI_WaitKey(terminalId, 5, ref key, ref ret);

                        ClassPOSPGW.PTI_Display(terminalId, "F. Administrativa nao imp : \rPRESSIONE UMA TECLA", ref ret);

                        //Usa função de aguardar tecla para deixar mensagem anterior na tela por 5 segundos:
                        ClassPOSPGW.PTI_WaitKey(terminalId, 5, ref key, ref ret);

                    }
                    else if (selectedOption == 3) // Retorna
                    {

                        return false;
                    }
                }
            }
            
        }


        public bool MenuImpressao(string terminalId)
        {

            short key = 99;
            short ret = 99;
            short selectedOption = -1;
            System.Text.StringBuilder amount = new System.Text.StringBuilder(30);

            while (true)
            {
                //Inicia função de menu:
                ClassPOSPGW.PTI_StartMenu(terminalId, ref ret);


                //Adiciona opção 1 ao menu:
                ClassPOSPGW.PTI_AddMenuOption(terminalId, "PrintReceipt", ref ret);
                //Adiciona opção 2 ao menu:
                ClassPOSPGW.PTI_AddMenuOption(terminalId, "Display", ref ret);
                //Adiciona opção 3 ao menu:
                ClassPOSPGW.PTI_AddMenuOption(terminalId, "Print", ref ret);
                //Adiciona opção 4 ao menu:
                ClassPOSPGW.PTI_AddMenuOption(terminalId, "PrnSymbolCode", ref ret);
                //Adiciona opção 5 ao menu:
                ClassPOSPGW.PTI_AddMenuOption(terminalId, "Retorna", ref ret);



                //Executa o menu:
                ClassPOSPGW.PTI_ExecMenu(terminalId, "SELECIONE A OPCAO", 30, ref selectedOption, ref ret);

                if (ret == (int)ClassPOSPGW.PTIRET.PTIRET_TIMEOUT)
                {
                    ClassPOSPGW.PTI_Display(terminalId, "Tempo de Espera \r  Esgotado(TIMEOUT)", ref ret);
                    // Usa função de aguardar tecla para deixar mensagem anterior na tela por 5 segundos:
                    ClassPOSPGW.PTI_WaitKey(terminalId, 5, ref key, ref ret);
                    return false;

                }
                else
                {
                    if (selectedOption == 0) // Imprime Recibo
                    {
                        ClassPOSPGW.PTI_EFT_PrintReceipt(terminalId, 3, ref ret);

                        //testar o retorno e imprimir mensagem de acordo.

                        if (ret != (short)ClassPOSPGW.PTIRET.PTIRET_OK)
                        {
                            ErrorMessage(ret, terminalId);
                        }

                        ClassPOSPGW.PTI_Beep(terminalId, 0, ref ret);

                    }
                    else if (selectedOption == 1) // Imprime na tela
                    {
                        //Usa função de aguardar tecla para deixar mensagem anterior na tela por 5 segundos:
                        ClassPOSPGW.PTI_WaitKey(terminalId, 5, ref key, ref ret);

                        ClassPOSPGW.PTI_Display(terminalId, "PTI_Display nao imp. : \rPRESSIONE UMA TECLA", ref ret);

                        //Usa função de aguardar tecla para deixar mensagem anterior na tela por 5 segundos:
                        ClassPOSPGW.PTI_WaitKey(terminalId, 5, ref key, ref ret);


                    }
                    else if (selectedOption == 2) // Imprime na impressora
                    {
                        //Usa função de aguardar tecla para deixar mensagem anterior na tela por 5 segundos:
                        ClassPOSPGW.PTI_WaitKey(terminalId, 5, ref key, ref ret);

                        ClassPOSPGW.PTI_Display(terminalId, "PTI_Print nao imp : \rPRESSIONE UMA TECLA", ref ret);

                        //Usa função de aguardar tecla para deixar mensagem anterior na tela por 5 segundos:
                        ClassPOSPGW.PTI_WaitKey(terminalId, 5, ref key, ref ret);

                    }
                    else if (selectedOption == 3) // imprime codigo de barras ou qr code
                    {
                        //Usa função de aguardar tecla para deixar mensagem anterior na tela por 5 segundos:
                        ClassPOSPGW.PTI_WaitKey(terminalId, 5, ref key, ref ret);

                        ClassPOSPGW.PTI_Display(terminalId, "PTI_PrnSymbolCode nao imp : \rPRESSIONE UMA TECLA", ref ret);

                        //Usa função de aguardar tecla para deixar mensagem anterior na tela por 5 segundos:
                        ClassPOSPGW.PTI_WaitKey(terminalId, 5, ref key, ref ret);

                    }
                    else if (selectedOption == 4) // Retorna
                    {
                        return false;
                    }
                }
            }
        }

        
        public bool MenuCaptura(string terminalId)
        {

            short key = 99;
            short ret = 99;
            short selectedOption = -1;
            

            

            while (true)
            {
                //Inicia função de menu:
                ClassPOSPGW.PTI_StartMenu(terminalId, ref ret);

                //Adiciona opção 1 ao menu:
                ClassPOSPGW.PTI_AddMenuOption(terminalId, "Dados Mascarados", ref ret);
                //Adiciona opção 2 ao menu:
                ClassPOSPGW.PTI_AddMenuOption(terminalId, "Dados Nao Masc.", ref ret);
                //Adiciona opção 3 ao menu:
                ClassPOSPGW.PTI_AddMenuOption(terminalId, "Retorna", ref ret);


                //Executa o menu:
                ClassPOSPGW.PTI_ExecMenu(terminalId, "SELECIONE A OPCAO", 30, ref selectedOption, ref ret);

                if (ret == (int)ClassPOSPGW.PTIRET.PTIRET_TIMEOUT)
                {
                    ClassPOSPGW.PTI_Display(terminalId, "Tempo de Espera \r  Esgotado(TIMEOUT)", ref ret);
                    // Usa função de aguardar tecla para deixar mensagem anterior na tela por 5 segundos:
                    ClassPOSPGW.PTI_WaitKey(terminalId, 5, ref key, ref ret);
                    return false;

                }
                else
                {
                    if (selectedOption == 0) // Dados Masc.
                    {
                        //Usa função de aguardar tecla para deixar mensagem anterior na tela por 5 segundos:
                        ClassPOSPGW.PTI_WaitKey(terminalId, 5, ref key, ref ret);

                        ClassPOSPGW.PTI_Display(terminalId, "Dados Mascarados nao imp. : \rPRESSIONE UMA TECLA", ref ret);

                        //Usa função de aguardar tecla para deixar mensagem anterior na tela por 5 segundos:
                        ClassPOSPGW.PTI_WaitKey(terminalId, 5, ref key, ref ret);


                    }
                    else if (selectedOption == 1) // Dados Nao Masc.
                    {
                        //Usa função de aguardar tecla para deixar mensagem anterior na tela por 5 segundos:
                        ClassPOSPGW.PTI_WaitKey(terminalId, 5, ref key, ref ret);

                        ClassPOSPGW.PTI_Display(terminalId, "Dados Nao Mascarados : \rPRESSIONE UMA TECLA", ref ret);

                        //Usa função de aguardar tecla para deixar mensagem anterior na tela por 5 segundos:
                        ClassPOSPGW.PTI_WaitKey(terminalId, 5, ref key, ref ret);

                    }
                    else if (selectedOption == 2) // Dados Nao Masc.
                    {
                        return false;
                    }
                }
            }
            
        }

        // Executa a transação de venda
        public void OperacaoVenda(string terminalId)
        {
            short ret = 99;            
            System.Text.StringBuilder amount = new System.Text.StringBuilder(30);

            ////////////
            //Obtém do usuário o valor da transação:
            ClassPOSPGW.PTI_GetData(terminalId, "DIGITE O VALOR DO PAGAMENTO:\r", "@@@.@@@,@@", 3, 8, false, false, false, 30, amount, 2, ref ret);

            //Inicia transação de pagamento:
            ClassPOSPGW.PTI_EFT_Start(terminalId, (int)ClassPOSPGW.PWOPER.PWOPER_SALE, ref ret);
            //Insere parâmetro "moeda":
            ClassPOSPGW.PTI_EFT_AddParam(terminalId, (int)ClassPOSPGW.PWINFO.PWINFO_CURRENCY, "986", ref ret);
            //Insere parâmetro "valor":
            ClassPOSPGW.PTI_EFT_AddParam(terminalId, (int)ClassPOSPGW.PWINFO.PWINFO_TOTAMNT, amount.ToString(), ref ret);
            //Executa transação:
            ClassPOSPGW.PTI_EFT_Exec(terminalId, ref ret);

            if (ret == 0)// Transação autorizada
            {
                //Impressão do comprovante da transação:
                ClassPOSPGW.PTI_EFT_PrintReceipt(terminalId, 3, ref ret);


                OperacaoConfirmacao(terminalId); // confirmação da ransação de venda
                ////////////

            }
            else // mensagem de erro
            {
               ErrorMessage(ret, terminalId); 
            }
        }
        
        // Confirma a última Transação realizada.  
        public void OperacaoConfirmacao(string terminalId)
        {
            short key = 99;
            short ret = 99;
            short selectedOption = -1;

            // Monta Menu de Confirmação
            ClassPOSPGW.PTI_StartMenu(terminalId, ref ret);
            ClassPOSPGW.PTI_AddMenuOption(terminalId, "SIM", ref ret);
            ClassPOSPGW.PTI_AddMenuOption(terminalId, "NAO", ref ret);
            ClassPOSPGW.PTI_ExecMenu(terminalId, "CONFIRMA TRANSACAO?", 30, ref selectedOption, ref ret);

            if (selectedOption == 0) // Confirma a Transação
            {
                ClassPOSPGW.PTI_EFT_Confirm(terminalId, (short)ClassPOSPGW.PTICNF.PTICNF_SUCCESS, ref ret);
                ClassPOSPGW.PTI_Display(terminalId, "Transacao Confirmada : \rPRESSIONE UMA TECLA", ref ret);

                //Usa função de aguardar tecla para deixar mensagem anterior na tela por 5 segundos:
                ClassPOSPGW.PTI_WaitKey(terminalId, 5, ref key, ref ret);

            }
            else // Nao Confirma a transação
            {
                ClassPOSPGW.PTI_EFT_Confirm(terminalId, (short)ClassPOSPGW.PTICNF.PTICNF_OTHERERR, ref ret);
                //ClassPOSPGW.PTI_EFT_Confirm(terminalId, (short)ClassPOSPGW.PTICNF.PTICNF_SUCCESS, ref ret);
                ClassPOSPGW.PTI_Display(terminalId, "Transacao Nao Confirmada : \rPRESSIONE UMA TECLA", ref ret);

            }

            ////////////

        }



        public void ImpressaoPTI_EFT_PrintReceipt(string terminalId)
        {
            short ret = 99;

            ClassPOSPGW.PTI_EFT_PrintReceipt(terminalId, 3, ref ret);

            //testar o retorno e imprimir mensagem de acordo.

            if (ret != (short)ClassPOSPGW.PTIRET.PTIRET_OK)
            {
                ErrorMessage(ret, terminalId);
        }

            ClassPOSPGW.PTI_Beep(terminalId, 0, ref ret);

        }

        // Operação de cancelamento
        public void Cancelamento(string terminalId)
        {
            short ret = 99;
            short key = 99;
         
           
            //Inicia transação de Cancelamento:
            ClassPOSPGW.PTI_EFT_Start(terminalId, (int)ClassPOSPGW.PWOPER.PWOPER_SALEVOID, ref ret);

            if (ret == (int)ClassPOSPGW.PTIRET.PTIRET_CANCEL)
            {
                            
               ClassPOSPGW.PTI_Display(terminalId, "Cancelado pela aplicacao: ", ref ret);
                // Usa função de aguardar tecla para deixar mensagem anterior na tela por 5 segundos:
                ClassPOSPGW.PTI_WaitKey(terminalId, 5, ref key, ref ret);
               return;
            }


            //Executa transação:
            ClassPOSPGW.PTI_EFT_Exec(terminalId, ref ret);

            ///////
            if (ret == 0)           // Transação autorizada OK
            {

                // Impressão do comprovante da transação:
                ClassPOSPGW.PTI_EFT_PrintReceipt(terminalId, 3, ref ret);

                // sinal sonoro
                ClassPOSPGW.PTI_Beep(terminalId, 0, ref ret);

                OperacaoConfirmacao(terminalId);

                ///////


            }  


        }
        public void ErrorMessage(int iIdMessage, string terminalId)
        {
            short key = 99;
            short ret = 99;

            string msg;
            msg = PTIRET_MSG[iIdMessage] + "\rPRESSIONE UMA TECLA";
            ClassPOSPGW.PTI_Display(terminalId, msg , ref ret);

            //Usa função de aguardar tecla para deixar mensagem anterior na tela por 5 segundos:
            ClassPOSPGW.PTI_WaitKey(terminalId, 5, ref key, ref ret);
        }
    }
}
