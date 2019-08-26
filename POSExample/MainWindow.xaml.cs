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
            short selectedOption = -1;
            ushort status = 99;
            System.Text.StringBuilder amount = new System.Text.StringBuilder(30);
            System.Text.StringBuilder mac = new System.Text.StringBuilder(30);
            System.Text.StringBuilder model = new System.Text.StringBuilder(30);
            System.Text.StringBuilder serialnumber = new System.Text.StringBuilder(50);

            //Mostra ao usuário o identificador do terminal que conectou:
            ClassPOSPGW.PTI_Display(terminalId, "TERMINAL\r" + terminalId + "\rCONECTADO", ref ret);

            //Usa função de aguardar tecla para deixar mensagem anterior na tela por 5 segundos:
            ClassPOSPGW.PTI_WaitKey(terminalId, 5, ref key, ref ret);

            //Consulta informações do terminal através da função PTI_CheckStatus:
            ClassPOSPGW.PTI_CheckStatus(terminalId, ref status, model, mac, serialnumber, ref ret);

            //Mostra ao usuário os dados obtidos através da função PTI_CheckStatus:
            ClassPOSPGW.PTI_Display(terminalId, "SERIAL: " + serialnumber + "\rMAC: " + mac + "\rMODELO: " + model, ref ret);

            //Usa função de aguardar tecla para deixar mensagem anterior na tela por 5 segundos:
            ClassPOSPGW.PTI_WaitKey(terminalId, 5, ref key, ref ret);

            ClassPOSPGW.PTI_Display(terminalId, "PRESSIONE UMA TECLA", ref ret);

            //Usa função de aguardar tecla para deixar mensagem anterior na tela por 5 segundos:
            ClassPOSPGW.PTI_WaitKey(terminalId, 5, ref key, ref ret);

            //Mostra ao usuário a tecla pressionada:
            ClassPOSPGW.PTI_Display(terminalId, "TECLA PRESSIONADA: " + key, ref ret);

            //Usa função de aguardar tecla para deixar mensagem anterior na tela por 5 segundos:
            ClassPOSPGW.PTI_WaitKey(terminalId, 5, ref key, ref ret);

            //Inicia função de menu:
            ClassPOSPGW.PTI_StartMenu(terminalId, ref ret);
            //Adiciona opção 1 do menu:
            ClassPOSPGW.PTI_AddMenuOption(terminalId, "OPCAO 1", ref ret);
            //Adiciona opção 2 ao menu:
            ClassPOSPGW.PTI_AddMenuOption(terminalId, "OPCAO 2", ref ret);
            //Executa o menu:
            ClassPOSPGW.PTI_ExecMenu(terminalId, "SELECIONE A OPCAO", 30, ref selectedOption, ref ret);

            if(selectedOption == 190)
            {
                //Mostra para o usuário que nenhuma opção foi selecionada:
                ClassPOSPGW.PTI_Display(terminalId, "NENHUMA OPCAO \rSELECIONADA", ref ret);
            }
            else
            {
                //Mostra para o usuário a opção selecionada por ele:
                ClassPOSPGW.PTI_Display(terminalId, "OPCAO SELECIONADA: " + selectedOption, ref ret);
            }

            //Usa função de aguardar tecla para deixar mensagem anterior na tela por 5 segundos:
            ClassPOSPGW.PTI_WaitKey(terminalId, 5, ref key, ref ret);

            //Obtém do usuário o valor da transação:
            ClassPOSPGW.PTI_GetData(terminalId, "DIGITE O VALOR DO PAGAMENTO", "@@@.@@@,@@", 3, 8, false, false, false, 30, amount, 2, ref ret);

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

                try
                {
                    List<string> responseObject = new List<string>();
                    uint[] responseFields = Array.ConvertAll((uint[])(Enum.GetValues(typeof(ClassPOSPGW.PWINFO))), Convert.ToUInt32);
                    foreach (int field in responseFields)
                    {

                        int intStringBuilderSize = 32000;
                        StringBuilder sb = new StringBuilder(intStringBuilderSize);
                        string str = "";
                        ClassPOSPGW.PTI_EFT_GetInfo(terminalId, field, intStringBuilderSize, sb, ref ret);
                        str = sb.ToString();

                        responseObject.Add(Enum.GetName(typeof(ClassPOSPGW.PWINFO), field) + " = " + str);

                    }

                    string transactionData = string.Join("\r", responseObject);

                    //Impressão de texto:
                    ClassPOSPGW.PTI_Print(terminalId, transactionData.Substring(0,700), ref ret);

                    //Imppressão de código de barras:
                    ClassPOSPGW.PTI_PrnSymbolCode(terminalId, "0123456789", 2, ref ret);

                    //Impressão de QR Code
                    ClassPOSPGW.PTI_PrnSymbolCode(terminalId, "http://www.ntk.com.br", 4, ref ret);

                    //Feed de papel:
                    ClassPOSPGW.PTI_PrnFeed(terminalId, ref ret);

                }
                catch (Exception){ }

                //Impressão do comprovante da transação:
                ClassPOSPGW.PTI_EFT_PrintReceipt(terminalId, 3, ref ret);

                ClassPOSPGW.PTI_Beep(terminalId, 0, ref ret);

                //Exemplo de menu:
                ClassPOSPGW.PTI_StartMenu(terminalId, ref ret);
                ClassPOSPGW.PTI_AddMenuOption(terminalId, "SIM", ref ret);
                ClassPOSPGW.PTI_AddMenuOption(terminalId, "NAO", ref ret);
                ClassPOSPGW.PTI_ExecMenu(terminalId, "CONFIRMA TRANSACAO?", 30, ref selectedOption, ref ret);

                if (selectedOption == 0)
                {
                    ClassPOSPGW.PTI_EFT_Confirm(terminalId, (short)ClassPOSPGW.PTICNF.PTICNF_SUCCESS, ref ret);
                }
                else
                {
                    ClassPOSPGW.PTI_EFT_Confirm(terminalId, (short)ClassPOSPGW.PTICNF.PTICNF_OTHERERR, ref ret);
                }

            }
            else
            {
                ClassPOSPGW.PTI_Beep(terminalId, 1, ref ret);
            }

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
    }
}
