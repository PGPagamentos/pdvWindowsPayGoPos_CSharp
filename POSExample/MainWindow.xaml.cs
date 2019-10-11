using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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


                WriteLog("PTI_Init ");

                WriteLog("");

                WriteLog("pszPOS_Company......: " + appCompany);
                WriteLog("pszPOS_Version......: " + appVersion);
                WriteLog("pszDataFolder.......: " + appWorkingPath);
                WriteLog("uiTCP_Port..........: " + appListeningPort.ToString());
                WriteLog("uiMaxTerminals......: " + maxNumberOfTerminals.ToString());
                






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
                        OperacaoAdmin(terminalId);

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
                  
                        ImpressaoPTI_EFT_PrintReceipt(terminalId);

                        ClassPOSPGW.PTI_Beep(terminalId, 0, ref ret);

                    }                  
                    else if (selectedOption == 1) // Imprime na tela
                    {
                        //Usa função de aguardar tecla para deixar mensagem anterior na tela por 5 segundos
                        
                        ClassPOSPGW.PTI_Display(terminalId, "Exemplo PTI_Display\r", ref ret);

                        //Usa função de aguardar tecla para deixar mensagem anterior na tela por 5 segundos:
                        ClassPOSPGW.PTI_WaitKey(terminalId, 5, ref key, ref ret);
                    }
                    else if (selectedOption == 2) // Imprime na impressora
                    {
                        System.Text.StringBuilder numero = new System.Text.StringBuilder(30);
                        // Captura um Numero
                        ClassPOSPGW.PTI_GetData(terminalId, "Numero Para Imprimir:\r", "@@@@@@", 3, 8, false, false, false, 30, numero, 2, ref ret);
                        string sPrint = "";

                        // Numero Digitado:
                        sPrint =  "PTI_Print Informado: " + numero.ToString();
                        // Impressão do Numero:
                        ClassPOSPGW.PTI_Print(terminalId, sPrint, ref ret);

                        // Avança Papel na Impressora
                        ClassPOSPGW.PTI_PrnFeed(terminalId, ref ret);
                        ClassPOSPGW.PTI_PrnFeed(terminalId, ref ret);

                    }
                    else if (selectedOption == 3) // imprime codigo de barras ou qr code
                    {
                        MenuImpressaoPrnSymbolCode(terminalId);

                    }
                    else if (selectedOption == 4) // Retorna
                    {
                        return false;
                    }
                }
            }
        }

        public bool MenuImpressaoPrnSymbolCode(string terminalId)
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
                ClassPOSPGW.PTI_AddMenuOption(terminalId, "QR Code", ref ret);
                //Adiciona opção 2 ao menu:
                ClassPOSPGW.PTI_AddMenuOption(terminalId, "Codigo Barras", ref ret);
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
                    if (selectedOption == 0) // Imprime QRCode
                    {
                        

                        ClassPOSPGW.PTI_PrnSymbolCode(terminalId, "http://www.ntk.com.br", 4, ref ret);

                        //Avança algumas linhas do papel da impressora:
                        ClassPOSPGW.PTI_PrnFeed(terminalId, ref ret);


                        ClassPOSPGW.PTI_Beep(terminalId, 0, ref ret);

                    }
                    else if (selectedOption == 1) // Imprime Codigo de Barras
                    {
                        ClassPOSPGW.PTI_PrnSymbolCode(terminalId, "0123456789", 2, ref ret);

                        //Avança algumas linhas do papel da impressora:
                        ClassPOSPGW.PTI_PrnFeed(terminalId, ref ret);


                        ClassPOSPGW.PTI_Beep(terminalId, 0, ref ret);


                    }
                    else if (selectedOption == 2) // Retorna
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
            

            WriteLog("\n---------------------------------");
            WriteLog("\nCaptura de Dados:\n");
            

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

                        System.Text.StringBuilder pszData = new System.Text.StringBuilder(30);
                        ClassPOSPGW.PTI_GetData(terminalId, "CPF C/Mascara\r" , "@@@.@@@.@@@-@@", 11, 11, false, false, true, 30, pszData, 2, ref ret);
                        string sData= pszData.ToString();
                        WriteLog("\n");
                        WriteLog("CPF Com Mascara Capturado: " + sData + " - Terminal: " + terminalId);
                        WriteLog("\n");
                        //Usa função de aguardar tecla para deixar mensagem anterior na tela por 5 segundos:
                        ClassPOSPGW.PTI_WaitKey(terminalId, 5, ref key, ref ret);


                    }
                    else if (selectedOption == 1) // Dados Nao Masc.
                    {

                        System.Text.StringBuilder pszData = new System.Text.StringBuilder(30);
                        ClassPOSPGW.PTI_GetData(terminalId, "CPF S/Mascara\r", "@@@.@@@.@@@-@@", 11, 11, true, false, false, 30, pszData, 2, ref ret);
                        string sData = pszData.ToString();
                        WriteLog("\n");
                        WriteLog("CPF Sem Mascara Capturado: " + sData + " - Terminal: " + terminalId);
                        WriteLog("\n");
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

                PrintResultParams(terminalId);
                //Impressão do comprovante da transação:
                //ClassPOSPGW.PTI_EFT_PrintReceipt(terminalId, 3, ref ret);
                ImpressaoPTI_EFT_PrintReceipt(terminalId);

                OperacaoConfirmacao(terminalId); // confirmação da ransação de venda
                ////////////

            }
            else // mensagem de erro
            {
                ErrorMessage(ret, terminalId);
                // Transação Pendente ou Falhou

                // Mostra valores armazenados em todos PWINFO_???
                
                PrintResultParams(terminalId);
                
               // metodo de confirmação para o caso de pendente ou falhou

                OperacaoConfirmacao(terminalId);


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
            short key = 99;

            ClassPOSPGW.PTI_EFT_PrintReceipt(terminalId, 3, ref ret);

            //testar o retorno e imprimir mensagem de acordo.

        
            if (ret == (short)ClassPOSPGW.PTIRET.PTIRET_NODATA)
            {
                WriteLog("\n");
                WriteLog("Não Existe Recibo a ser Impresso");
                ClassPOSPGW.PTI_Display(terminalId, "Nao Existe Recibo: \r", ref ret);
                // Usa função de aguardar tecla para deixar mensagem anterior na tela por 5 segundos:
                ClassPOSPGW.PTI_WaitKey(terminalId, 5, ref key, ref ret);
            }
            else if (ret == (short)ClassPOSPGW.PTIRET.PTIRET_NOPAPER)
            {
                WriteLog("\n");
                WriteLog("Impressora sem Papel\n");
                ClassPOSPGW.PTI_Display(terminalId, "Impressora sem Papel: \r", ref ret);
                // Usa função de aguardar tecla para deixar mensagem anterior na tela por 5 segundos:
                ClassPOSPGW.PTI_WaitKey(terminalId, 5, ref key, ref ret);
            }
            else if (ret != (short)ClassPOSPGW.PTIRET.PTIRET_OK)
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
                PrintResultParams(terminalId);
                // Impressão do comprovante da transação:
                //ClassPOSPGW.PTI_EFT_PrintReceipt(terminalId, 3, ref ret);

                ImpressaoPTI_EFT_PrintReceipt(terminalId);
                // sinal sonoro
                ClassPOSPGW.PTI_Beep(terminalId, 0, ref ret);

                OperacaoConfirmacao(terminalId);

                ///////


            }
            else
            {
                PrintResultParams(terminalId);
                OperacaoConfirmacao(terminalId);
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

        
        // Operação  administrativa
        public void OperacaoAdmin(string terminalId)
        {
            short ret = 99;
            short key = 99;


            //Inicia transação de Cancelamento:
            ClassPOSPGW.PTI_EFT_Start(terminalId, (int)ClassPOSPGW.PWOPER.PWOPER_ADMIN, ref ret);

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

                PrintResultParams(terminalId);
                ClassPOSPGW.PTI_Display(terminalId, "Operacao Administrativa OK: ", ref ret);


            }
            else
            {                             
                // Impressão do comprovante da transação:
                //                ClassPOSPGW.PTI_EFT_PrintReceipt(terminalId, 3, ref ret);

                // sinal sonoro
                //              ClassPOSPGW.PTI_Beep(terminalId, 0, ref ret);
                PrintResultParams(terminalId);
                OperacaoConfirmacao(terminalId);

                ///////


            }


        }

        ///////////////////////////////
        /*
        //=====================================================================================
             Funcao     :  PrintResultParams

             Descricao  :  Esta função exibe na tela todas as informações de resultado disponíveis
                           no momento em que foi chamada.

             Entradas   :  nao ha.

             Saidas     :  nao ha.

             Retorno    :  nao ha.
          }
        =====================================================================================
        ////////////////////////////
        */

        public void PrintResultParams(string terminalId)
        {
            short ret = 99;
            //short key = 99;

            int I=0;
            //int Ir=0;
            string volta="";

            //int iRet;
            string retorno="";
            //string retornoMemo="";
            string WTexto="";
            string WTextoMemo="";
            int Wmax=0;

            //char[] pszValue = new char[2048];

            //Inicia transação de Cancelamento:
            //         ClassPOSPGW.PTI_EFT_Start(terminalId, (int)ClassPOSPGW.PWOPER.PWOPER_ADMIN, ref ret);


            //////////////////////
            I = 0;
            WTexto = "";
            Wmax = 32000;

            while (I < Wmax)
            {

                volta= pszGetInfoDescription(I);
                if (volta == "PWINFO_XXX")
                {
                    I= I + 1;
                    continue;
                }


                /*
                      CPT_GetpszValue = record
                            pszValue: Array[0..2048] of AnsiChar;
                       end;
                       PSZ_GetpszValue = Array[0..0] of CPT_GetpszValue; 
                */

                StringBuilder pszValue =  new StringBuilder(2048);
                


                ClassPOSPGW.PTI_EFT_GetInfo(terminalId, I, 2048, pszValue, ref ret);

                if (ret == (int)ClassPOSPGW.PTIRET.PTIRET_OK)
                {
                     
                    //string str = new string(pszValue);
                    retorno = pszValue.ToString();
                    WTexto  = WTexto + volta + " = " + retorno + "\n";
                    //WTextoMemo = WTextoMemo + volta + " = " + retorno + "\n" + "\r";
                    WTextoMemo = WTextoMemo + volta + " = " + retorno + "\n";
            }

                I= I + 1;

        }

            WriteLog(WTextoMemo);



            /////////////////////////////





        }

        public void WriteLog(string sLine)
        {


            string _input = sLine;
            ////////////////////////////
            using (StringReader reader = new StringReader(_input))
            {
                // Loop over the lines in the string.
                //int count = 0;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    //count++;
                    //Console.WriteLine("Line {0}: {1}", count, line);
                    
                    this.Dispatcher.Invoke(() =>
                    {
                        this.lstBoxLog.Items.Add(line);
                    });
                }
            }
            

        }


        public string pszGetInfoDescription(int wIdentificador)
        {
            switch (wIdentificador)
            {
                case (int)ClassPOSPGW.PWINFO.PWINFO_OPERATION       : return "PWINFO_OPERATION";
                case (int)ClassPOSPGW.PWINFO.PWINFO_MERCHANTCNPJCPF : return "PWINFO_MERCHANTCNPJCPF";
                case (int)ClassPOSPGW.PWINFO.PWINFO_TOTAMNT         : return "PWINFO_TOTAMNT";
                case (int)ClassPOSPGW.PWINFO.PWINFO_CURRENCY        : return "PWINFO_CURRENCY";
                case (int)ClassPOSPGW.PWINFO.PWINFO_FISCALREF       : return "PWINFO_FISCALREF";
                case (int)ClassPOSPGW.PWINFO.PWINFO_CARDTYPE        : return "PWINFO_CARDTYPE";
                case (int)ClassPOSPGW.PWINFO.PWINFO_PRODUCTNAME     : return "PWINFO_PRODUCTNAME";
                case (int)ClassPOSPGW.PWINFO.PWINFO_DATETIME        : return "PWINFO_DATETIME";
                case (int)ClassPOSPGW.PWINFO.PWINFO_REQNUM          : return "PWINFO_REQNUM";
                case (int)ClassPOSPGW.PWINFO.PWINFO_AUTHSYST        : return "PWINFO_AUTHSYST";
                case (int)ClassPOSPGW.PWINFO.PWINFO_VIRTMERCH       : return "PWINFO_VIRTMERCH";
                case (int)ClassPOSPGW.PWINFO.PWINFO_AUTMERCHID      : return "PWINFO_AUTMERCHID";
                case (int)ClassPOSPGW.PWINFO.PWINFO_FINTYPE         : return "PWINFO_FINTYPE";
                case (int)ClassPOSPGW.PWINFO.PWINFO_INSTALLMENTS    : return "PWINFO_INSTALLMENTS";
                case (int)ClassPOSPGW.PWINFO.PWINFO_INSTALLMDATE    : return "PWINFO_INSTALLMDATE";
                case (int)ClassPOSPGW.PWINFO.PWINFO_RESULTMSG       : return "PWINFO_RESULTMSG";
                case (int)ClassPOSPGW.PWINFO.PWINFO_AUTLOCREF       : return "PWINFO_AUTLOCREF";
                case (int)ClassPOSPGW.PWINFO.PWINFO_AUTEXTREF       : return "PWINFO_AUTEXTREF";
                case (int)ClassPOSPGW.PWINFO.PWINFO_AUTHCODE        : return "PWINFO_AUTHCODE";
                case (int)ClassPOSPGW.PWINFO.PWINFO_AUTRESPCODE     : return "PWINFO_AUTRESPCODE";
                case (int)ClassPOSPGW.PWINFO.PWINFO_DISCOUNTAMT     : return "PWINFO_DISCOUNTAMT";
                case (int)ClassPOSPGW.PWINFO.PWINFO_CASHBACKAMT     : return "PWINFO_CASHBACKAMT";
                case (int)ClassPOSPGW.PWINFO.PWINFO_CARDNAME        : return "PWINFO_CARDNAME";
                case (int)ClassPOSPGW.PWINFO.PWINFO_BOARDINGTAX     : return "PWINFO_BOARDINGTAX";
                case (int)ClassPOSPGW.PWINFO.PWINFO_TIPAMOUNT       : return "PWINFO_TIPAMOUNT";
                case (int)ClassPOSPGW.PWINFO.PWINFO_TRNORIGDATE     : return "PWINFO_TRNORIGDATE";
                case (int)ClassPOSPGW.PWINFO.PWINFO_TRNORIGNSU      : return "PWINFO_TRNORIGNSU";
                case (int)ClassPOSPGW.PWINFO.PWINFO_TRNORIGAUTH     : return "PWINFO_TRNORIGAUTH";
                case (int)ClassPOSPGW.PWINFO.PWINFO_LANGUAGE        : return "PWINFO_LANGUAGE";
                case (int)ClassPOSPGW.PWINFO.PWINFO_TRNORIGTIME     : return "PWINFO_TRNORIGTIME";
                case (int)ClassPOSPGW.PWINFO.PWPTI_RESULT           : return "PWPTI_RESULT";
                case (int)ClassPOSPGW.PWINFO.PWINFO_CARDENTMODE     : return "PWINFO_CARDENTMODE";
                case (int)ClassPOSPGW.PWINFO.PWINFO_CARDPARCPAN     : return "PWINFO_CARDPARCPAN";
                case (int)ClassPOSPGW.PWINFO.PWINFO_CHOLDVERIF      : return "PWINFO_CHOLDVERIF";
                case (int)ClassPOSPGW.PWINFO.PWINFO_MERCHADDDATA1   : return "PWINFO_MERCHADDDATA1";
                case (int)ClassPOSPGW.PWINFO.PWINFO_MERCHADDDATA2   : return "PWINFO_MERCHADDDATA2";
                case (int)ClassPOSPGW.PWINFO.PWINFO_MERCHADDDATA3   : return "PWINFO_MERCHADDDATA3";
                case (int)ClassPOSPGW.PWINFO.PWINFO_MERCHADDDATA4   : return "PWINFO_MERCHADDDATA4";
                case (int)ClassPOSPGW.PWINFO.PWINFO_PNDAUTHSYST     : return "PWINFO_PNDAUTHSYST";
                case (int)ClassPOSPGW.PWINFO.PWINFO_PNDVIRTMERCH    : return "PWINFO_PNDVIRTMERCH";
                case (int)ClassPOSPGW.PWINFO.PWINFO_PNDAUTLOCREF    : return "PWINFO_PNDAUTLOCREF";
                case (int)ClassPOSPGW.PWINFO.PWINFO_PNDAUTEXTREF    : return "PWINFO_PNDAUTEXTREF";
                case (int)ClassPOSPGW.PWINFO.PWINFO_DUEAMNT         : return "PWINFO_DUEAMNT";
                case (int)ClassPOSPGW.PWINFO.PWINFO_READJUSTEDAMNT  : return "PWINFO_READJUSTEDAMNT";
                default                                             : return "PWINFO_XXX";



            }
        }

    }
}
