using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace POSExample
{
    public class ClassPOSPGW
    {

        #region DLL

        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_AddMenuOption(string terminalId, string pszOption, [MarshalAs(UnmanagedType.I2)] ref short piRet);

        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_Beep(string terminalId, int iType, [MarshalAs(UnmanagedType.I2)] ref short piRet);

        [DllImport("PTI_DLL.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_CheckStatus(string pszTerminalId, [MarshalAs(UnmanagedType.U2)] ref ushort piStatus,
            StringBuilder pszModel, StringBuilder pszMAC, StringBuilder pszSerNo, [MarshalAs(UnmanagedType.I2)] ref short piRet);

        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_ConnectionLoop(StringBuilder pszTerminalId, StringBuilder pszModel, StringBuilder pszMAC, StringBuilder pszSerNo, ref short piRet);

        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_Disconnect(string uiTerminalId, short uiPwrDelay, [MarshalAs(UnmanagedType.I2)] ref short piRet);

        [DllImport("PTI_DLL.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_Display(string uiTerminalId, string pszMsg, [MarshalAs(UnmanagedType.I2)] ref short piRet);

        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_EFT_AddParam(string uiTerminalId, int iParam, string pszValue, [MarshalAs(UnmanagedType.I2)] ref short piRet);

        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_EFT_Confirm(string uiTerminalId, int iResult, [MarshalAs(UnmanagedType.I2)] ref short piRet);

        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_EFT_Exec(string uiTerminalId, [MarshalAs(UnmanagedType.I2)] ref short piRet);

        [DllImport("PTI_DLL.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_EFT_GetInfo(string uiTerminalId, int iInfo, int uiBufLen, StringBuilder pszValue, [MarshalAs(UnmanagedType.I2)] ref short piRet);

        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_EFT_PrintReceipt(string uiTerminalId, int iCopies, [MarshalAs(UnmanagedType.I2)] ref short piRet);

        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_EFT_Start(string uiTerminalId, int iOper, [MarshalAs(UnmanagedType.I2)] ref short piRet);

        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_End();

        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_ExecMenu(string terminalId, string pszPrompt, short uiTimeOutSec, ref short puiSelection, 
            [MarshalAs(UnmanagedType.I2)] ref short piRet);

        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_GetData(string uiTerminalId, string pszPrompt, string pszFormat, short uiLenMin,
            short uiLenMax, bool fFromLeft, bool fAlpha, bool fMask, short uiTimeOutSec, StringBuilder pszData, short uiCaptureLine, ref short piRet);

        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_Init(string pszPOS_Company, string pszPOS_Version, string pszPOS_Capabilities, string pszDataFolder,
                int uiTCP_Port, int uiMaxTerminals, string pszWaitMsg, int uiAutoDiscSec, [MarshalAs(UnmanagedType.I2)] ref short piRet);
        
        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_Print(string uiTerminalId, string text,
            [MarshalAs(UnmanagedType.I2)] ref short piRet);

        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_PrnFeed(string uiTerminalId, [MarshalAs(UnmanagedType.I2)] ref short piRet);

        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_PrnSymbolCode(string uiTerminalId, string pszMsg, int iSymbology, [MarshalAs(UnmanagedType.I2)] ref short piRet);
        
        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_StartMenu(string terminalId, [MarshalAs(UnmanagedType.I2)] ref short piRet);

        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_WaitKey(string terminalId, short uiTimeOutSec,
            ref short piKey, [MarshalAs(UnmanagedType.I2)] ref short piRet);

        #endregion

        #region enums

        public enum PWINFO : uint
        {
            PWINFO_OPERATION = 2, //Transaction that was performed: “00” – not defined “01” – Sale (payment) “02” – Administrative (general) “04” – Sale void 
            PWINFO_MERCHANTCNPJCPF = 28,  //CNPJ (ou CPF) do Estabelecimento / Ponto de captura 
            PWINFO_TOTAMNT = 37, //Final transaction amount, in cents, with leading zeros. 
            PWINFO_CURRENCY = 38, //Currency code, according to ISO4217. 
            PWINFO_FISCALREF = 40, //Número da fatura (final nulo). Este parâmetro é opcional.
            PWINFO_CARDTYPE = 41, //Modalidade da transação do cartão: 1: crédito; 2: débito; 4: voucher; 8: private label; 16: frota; 128: outros. 
            PWINFO_PRODUCTNAME = 42, //Nome/tipo do produto utilizado, na nomenclatura do Provedor. 
            PWINFO_DATETIME = 49, //Transaction date and time, format “YYMMDDhhmmss”. 
            PWINFO_REQNUM = 50, //Unique transaction identifier (generated by the terminal). 
            PWINFO_AUTHSYST = 53, //Name of the acquirer/processor that processed the transaction. 
            PWINFO_VIRTMERCH = 54, //Merchant identifier for the terminal management system. 
            PWINFO_AUTMERCHID = 56, //Merchant identifier for the acquirer.
            PWINFO_FINTYPE = 59, //Modalidade de financiamento da transação: 1: à vista; 2: parcelado pelo Emissor; 4: parcelado pelo Estabelecimento; 8: pré-datado; 16: crédito emissor; 32: Prédatado parcelado. 
            PWINFO_INSTALLMENTS = 60, //Quantidade de parcelas, para transações parceladas.
            PWINFO_INSTALLMDATE = 61, //Data de vencimento do prédatado, ou da primeira parcela. Formato “DDMMAA”. 
            PWINFO_RESULTMSG = 66, //Error code (if PTI_EFT_Exec returns PTIRET_EFTERR).
            PWINFO_AUTLOCREF = 68, //Unique transaction identifier (generated by the terminal management system). 
            PWINFO_AUTEXTREF = 69, //Unique transaction identifier (generated by the acquirer/processor). 
            PWINFO_AUTHCODE = 70, //Authorization code (generated by the issuer). 
            PWINFO_AUTRESPCODE = 71, //Caso a transação chegue ao sistema autorizador, esse é o código de resposta do mesmo  (bit39 da mensagem ISO8583). 
            PWINFO_DISCOUNTAMT = 73, //Valor do desconto concedido pelo Provedor, considerando PWINFO_CURREXP, já deduzido em PWINFO_TOTAMNT. 
            PWINFO_CASHBACKAMT = 74, //Valor do saque/troco, considerando PWINFO_CURREXP, já incluído em PWINFO_TOTAMNT. 
            PWINFO_CARDNAME = 75, //Name of the card or issuer. 
            PWINFO_BOARDINGTAX = 77, //Valor da taxa de embarque, considerando PWINFO_CURREXP, já incluído em PWINFO_TOTAMNT. 
            PWINFO_TIPAMOUNT = 78, //Valor da taxa de serviço (gorjeta), considerando PWINFO_CURREXP, já incluído em PWINFO_TOTAMNT. 
            //PWINFO_RCPTFULL = 82, //Comprovante – via completa.
            PWINFO_RCPTMERCH = 83, //Comprovante – via do estabelecimento.
            PWINFO_RCPTCHOLDER = 84, //Comprovante – via do portador.
            PWINFO_RCPTCHSHORT = 85, //Comprovante – via reduzida 
            PWINFO_TRNORIGDATE = 87, //Data da transação original. Este campo é utilizado para transações de cancelamento. Formato DDMMAA. 
            PWINFO_TRNORIGNSU = 88, //Número de referência da transação original (atribuído pela adquirente/processadora). Este parâmetro é mandatório para transações de estorno (PTITRN_SALEVOID).
            PWINFO_TRNORIGAUTH = 98, //Código de autorização da transação original. Este campo é utilizado para transações de cancelamento.
            PWINFO_LANGUAGE = 108, //Idioma a ser utilizado para a interface com o cliente: 0: Português 1: Inglês 2: Espanhol 
            PWINFO_TRNORIGTIME = 115, //Horário da transação original. Este campo é utilizado para transações de cancelamento. Formato HHMMSS. 
            PWPTI_RESULT = 129, //Caso a execução da função retorne PTIRET_EFTERR, este campo informa o detalhamento do erro. 
            PWINFO_CARDENTMODE = 192, //Modo de entrada do cartão:  1: número do cartão digitado 2: tarja magnética 4: chip com contato EMV  16: fallback para tarja magnética  32: chip sem contato simulando tarja magnética 64: chip sem contato EMV 128: indica que a transação atual é oriunda de um fallback (flag enviado do servidor para o ponto de captura). 256: fallback de tarja para digitado 
            PWINFO_CARDPARCPAN = 200, //Número do cartão mascarado
            PWINFO_CHOLDVERIF = 207, //Verificação do portador do cartão, soma de:  1: assinatura 2: verificação offline da senha 4: senha offline bloqueada durante a transação  8: verificação on-line da senha.
            PWINFO_MERCHADDDATA1 = 240, //Número de referência da transação atribuído pela Automação Comercial. Caso fornecido, este número será incluído no histórico de dados da transação e encaminhado à adquirente/processadora, se suportado. Este parâmetro é opcional. 
            PWINFO_MERCHADDDATA2 = 241, //Dados adicionais específicos do negócio. Caso fornecido, será incluso no histórico de dados da transação, por exemplo para referências cruzadas. Este parâmetro é opcional.    
            PWINFO_MERCHADDDATA3 = 242, //Dados adicionais específicos do negócio. Caso fornecido, será incluso no histórico de dados da transação, por exemplo para referências cruzadas. Este parâmetro é opcional.  
            PWINFO_MERCHADDDATA4 = 243, //Dados adicionais específicos do negócio. Caso fornecido, será incluso no histórico de dados da transação, por exemplo para referências cruzadas. Este parâmetro é opcional. 
            PWINFO_PNDAUTHSYST = 32517, //Nome do provedor para o qual existe uma transação pendente. 
            PWINFO_PNDVIRTMERCH = 32518, //Identificador do Estabelecimento para o qual existe uma transação pendente. 
            PWINFO_PNDAUTLOCREF = 32520, //Referência para a infraestrutura Erro! Nome de propriedade do documento desconhecido. da transação que está pendente. 
            PWINFO_PNDAUTEXTREF = 32521, //Referência para o Provedor da transação que está pendente. 
            PWINFO_DUEAMNT = 48902, //Valor devido pelo usuário, considerando PWINFO_CURREXP, já deduzido em PWINFO_TOTAMNT. 
            PWINFO_READJUSTEDAMNT = 48905 //Valor total da transação reajustado, este campo será utilizado caso o autorizador, por alguma regra de negócio específica dele, resolva alterar o valor total que foi solicitado para a transação. 

        }

        public enum PTICNF : uint
        {
            PTICNF_SUCCESS = 1,// Transaction is confirmed. 
            PTICNF_PRINTERR = 2, //Printer error, reverse transaction. 
            PTICNF_DISPFAIL = 3,//Error with the dispenser mechanism, reverse transaction. 
            PTICNF_OTHERERR = 4 //Other error, reverse transaction. 
        }

        public enum PTIPRN : uint
        {
            PTIPRN_MERCHANT = 1,  //Merchant copy
            PTIPRN_CHOLDER = 2, //Cardholder copy
            PTIPRN_BOTH = 3 //Print both
        }

        public enum PTIRET : int
        {
            ERRO_INTERNO = 99, //Erro desta aplicação
            PTIRET_OK = 0, //Successful operation. 
            PTIRET_INVPARAM = -2001, //Invalid parameter passed to the function. 
            PTIRET_NOCONN = -2002, //The terminal is offline. 
            PTIRET_BUSY = -2003, //The terminal is busy processing another command. 
            PTIRET_TIMEOUT = -2004, //User failed to press a key during the specified time. 
            PTIRET_CANCEL = -2005, //User pressed the [CANCEL] key. 
            PTIRET_NODATA = 2006, //Required information is not available. 
            PTIRET_BUFOVRFLW = -2007, //Data is larger than the provided buffer size. 
            PTIRET_SOCKETERR = -2008, //Unable to start listening to the specified TCP ports. 
            PTIRET_WRITEERR = -2009, //Unable to use the specified directory. 
            PTIRET_EFTERR = -2010, //The EFT operation was completed, but failed. 
            PTIRET_INTERNALERR = -2011, //Integration library internal error.
            PTIRET_PROTOCOLERR = -2012, //Communication error between the integration library and the terminal.
            PTIRET_SECURITYERR = -2013, //The function failed for security reasons.
            PTIRET_PRINTERR = -2014, //Printer error. 
            PTIRET_NOPAPER = -2015, //Printer out of paper. 
            PTIRET_NEWCONN = -2016, //Novo terminal conectado.
            PTIRET_NONEWCONN = -2017, //Sem recebimento de novas conexões.
            PTIRET_NOTSUPPORTED = -2057, //Function not supported by the terminal. 
            PTIRET_CRYPTERR = -2058, //Data encryption error (communication between integration library and terminal). 
        }

        public enum PWOPER : uint
        {
            PWOPER_ADMIN = 32, //Any transaction that is not a payment (void, refund, preauthorization, report, receipt reprinting, etc.). 
            PWOPER_SALE = 33, //Payment of goods or services. 
            PWOPER_SALEVOID = 34 //Voids a sale transaction that was previously performed and confirmed.
        }

        public enum PWINFOFINTYPE : int
        {
            //Transaction financing mode:
            in_cash = 1,
            in_installments_issuer = 2,
            in_installments_merchant = 4,
            pre_dated = 8,
            credit_issuer = 16,
            pre_dated_in_installments = 32
        }

        public enum PWINFOCARDTYPE : int
        {
            Credit = 1,
            Debit = 2,
            Voucher = 4,
            Private_Label = 8,
            Fleet = 16,
            Others = 128
        }

        #endregion

    }
}
