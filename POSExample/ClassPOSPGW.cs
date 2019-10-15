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


        //===================================================================================================================================
         /*
            Function       :  PTI_AddMenuOption

            Descrição      :  Esta função adiciona uma opção ao menu que foi criado através de PTI_StartMenu.
                              Esta função retorna imediatamente.

            Entrada        :  pszTerminalId Identificador único do terminal (final nulo).
                              pszOption Mensagem de texto com final nulo que descreve a opção a ser exibida no
                                             terminal (máximo: 18 caracteres).

            Saida          :  none.

            Retorno        :  PTIRET_OK A opção foi adicionada ao menu
                       PTIRET_INVPARAM  Parâmetro inválido passado à função
                       PTIRET_NOCONN    O terminal está offline.
                       PTIRET_BUSY      O terminal está ocupado processando outro comando
        */
        //===================================================================================================================================
        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_AddMenuOption(string terminalId, string pszOption, [MarshalAs(UnmanagedType.I2)] ref short piRet);



        //==================================================================================================================
        /*
            Function       :  PTI_Beep

            Descrição      :  Esta função emite um aviso sonoro no terminal.Esta função retorna imediatamente.

            Entrada        :  pszTerminalId Identificador único do terminal (final nulo).
                              iType Tipo de aviso sonoro, de acordo com a tabela abaixo

            Saida          :  Nenhuma.

            Retorno        :  PTIRET_OK Operação bem-sucedida
                              PTIRET_INVPARAM    Parâmetro inválido passado à função
                              PTIRET_NOCONN      O terminal está offline
                              PTIRET_BUSY O terminal está ocupado processando outro comando
        */
        //==================================================================================================================
        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_Beep(string terminalId, int iType, [MarshalAs(UnmanagedType.I2)] ref short piRet);



        //===============================================================================================
        /*
             Function      :  PTI_CheckStatus

             Descricao     :  Esta função permite que a Automação Comercial verifique o status(on-line ou offline)
                              de determinado terminal de pagamento e recupere informações adicionais do equipamento.

                              Cada terminal de pagamento recebe um único identificador lógico, que é configurado quando o
                              terminal é instalado.Se a Automação Comercial controla mais de um terminal, ela deve ter registro
                              de todos os identificadores e suas localizações, com a finalidade de poder enviar comandos para o terminal desejado.


             Entrada       :  pszTerminalId Identificador único do terminal (final nulo). Pode ser vazio se o número máximo de terminais
                              suportado (informado em PTI_Init) for 1.


             Saida         :  pszTerminalId Identificador único do terminal(final nulo, até 20 caracteres).

                              piStatus Status do terminal(PTISTAT_xxx).

                              pszModel Modelo do terminal(final nulo até 20 caracteres).

                              pszMAC Endereço MAC do terminal(final nulo, formato “XX:XX:XX:XX:XX:XX”)

                              pszSerNo Número de série do terminal(final nulo, até 25 caracteres).

             Retorno      :  PTIRET_OK Operação bem sucedida.


        Lista de Possiveis Status(piStatus):
             ==================================

        Nome Valor     Descrição
        PTISTAT_IDLE        0       Terminal está on-line
        PTISTAT_BUSY        1       Terminal está on-line, porém ocupado processando um comando.
        PTISTAT_NOCONN      2       Terminal está offline.
        PTISTAT_WAITRECON   3       Terminal está off-line.A transação continua sendo executada e

                                    após sua finalização, o terminal tentará efetuar a reconexão

                                    automaticamente.
        */
        //===============================================================================================
        [DllImport("PTI_DLL.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_CheckStatus(string pszTerminalId, [MarshalAs(UnmanagedType.U2)] ref ushort piStatus,
            StringBuilder pszModel, StringBuilder pszMAC, StringBuilder pszSerNo, [MarshalAs(UnmanagedType.I2)] ref short piRet);




        //===========================================================================
        /*
            Funcao     :  PTI_ConnectionLoop

            Descricao  :  Esta função permite que a Automação Comercial verifique quando um novo
                          terminal se conectou e, se PTIRET_NEWCONN é retornado,
                          recupere informações adicionais do equipamento.

            Entradas   :  nenhuma.

            Saidas     :  pszTerminalId  : Identificador único do terminal(final nulo, até 20 caracteres).
                          pszModel       : Modelo do terminal(final nulo, até 20 caracteres).
                          pszMAC         : Endereço MAC do terminal(final nulo, formato “XX:XX:XX:XX:XX:XX”).
                          pszSerNo       : Número serial do terminal(final nulo, até 25 caracteres).


            Retorno    :  PTIRET_NEWCONN    :  Novo terminal conectado.
                          PTIRET_NONEWCONN  :  Sem novas conexões recebidas.
        */
        //===========================================================================
        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_ConnectionLoop(StringBuilder pszTerminalId, StringBuilder pszModel, StringBuilder pszMAC, StringBuilder pszSerNo, ref short piRet);


        //==============================================================================================
        /*
         Function      :  PTI_Disconnect

         Descricao     :  Esta função permite que a Automação Comercial desconecte um terminal de pagamento e o coloque
                          em modo offline, seja imediatamente ou após algum tempo funcionando sem alimentação externa.

                          Para terminais móveis, permanecer on-line aumenta consideravelmente o consumo da bateria.Por
                          este motivo é recomendado que a Automação Comercial defina um valor diferente de zero para o
                          parâmetro uiAutoDiscSec de PTI_Init, ou chame essa função assim que o terminal conectar.

                          Após o terminal ficar offline, tão logo uma tecla é pressionada, este se conecta automaticamente
                          novamente à Automação Comercial.


         Entrada       :  pszTerminalId Identificador único do terminal(final nulo).

                          uiPwrDelay Se igual a zero, desconecta imediatamente o terminal, independentemente
                                         de sua fonte de energia.
                                         Se diferente de zero, representa o número máximo de segundos durante os
                                         quais o terminal permanecerá on-line enquanto estiver operando sem
                                         alimentação externa.O terminal não ficará offline enquanto estiver
                                        conectado a uma fonte de alimentação externa.Este valor sobrescreve o
                                        parâmetro uiAutoDiscSec de PTI_Init para este terminal específico.

        Saida         :  none.

        Retorno       :  PTIRET_OK Operação bem-sucedida.
                         PTIRET_NOCONN O terminal está offline.
                         PTIRET_BUSY O terminal está ocupado processando outro comando
        */
        //===============================================================================================
        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_Disconnect(string uiTerminalId, short uiPwrDelay, [MarshalAs(UnmanagedType.I2)] ref short piRet);


          //===============================================================================================
          /*
            Function       :  PTI_Display

            Descricao      :  Esta função apresenta uma mensagem na tela do terminal e retorna imediatamente.
                              A mensagem é apresentada a partir do canto superior esquerdo da tela, sendo 20 caracteres por
                              linha, com quebra de linha identificada pelo caractere ‘\r’ (retorno ao início da linha, código
                              ASCII 13). Caracteres que ultrapassem as 20 colunas ou o número máximo de linhas são descartados.
                              O número máximo de linhas suportado pode variar dependendo do modelo do terminal, entretanto
                              o mínimo de quatro linhas é sempre suportado.

            Entrada        :  pszTerminalId Identificador único do terminal(final nulo).
                              pszMsg Mensagem a ser apresentada na tela do terminal(final nulo).

            Saida          :  none.

            Retorno        :  PTIRET_OK Operação bem-sucedida.
                   PTIRET_INVPARAM Parâmetro inválido passado à função.PTIRET

              PTIRET_NOCONN O terminal está offline
              PTIRET_BUSY          O terminal está ocupado processando outro comando

           */
        //===============================================================================================

        [DllImport("PTI_DLL.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_Display(string uiTerminalId, string pszMsg, [MarshalAs(UnmanagedType.I2)] ref short piRet);

        //===============================================================================================================================================
        /*
            Function       :  PTI_EFT_AddParam

            Descrição      :  A Automação Comercial deve chamar esta função iterativamente após PTI_EFT_Start para definir
                              todos os parâmetros disponíveis para a transação.Esta função retorna imediatamente.

            Entrada        :  pszTerminalId Identificador único do terminal (final nulo).


                         iParam Identificador do parâmetro, de acordo com o capítulo “TAG’s de entrada e saída”.


                         pszValue Valor do parâmetro (final nulo).


            Saida         :  none.

            Retorno       :  PTIRET_OK Successful operation.
                             PTIRET_INVPARAM Invalid parameter passed to the function.
                             PTIRET_NOCONN The terminal is offline.
                             PTIRET_BUSY The terminal is busy processing another command.
        */
        //===============================================================================================================================================
        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_EFT_AddParam(string uiTerminalId, int iParam, string pszValue, [MarshalAs(UnmanagedType.I2)] ref short piRet);

       

    //============================================================================================================================
    /*
        Function       :  PTI_EFT_Confirm

        Descrição      :  Qualquer transação financeira bem-sucedida(PTI_EFT_Exec retorna PTIRET_OK) deve ser
                          confirmada pela Automação Comercial através desta função para assegurar a integridade da
                          transação entre todas as partes(Automação Comercial e registro fiscais, terminal, adquirente,
                          emissor e portador do cartão).
                          Múltiplas transações podem ser realizadas simultaneamente por diversos terminais, entretanto, para
                          cada terminal, a transação deve ser confirmada antes de outra ser iniciada.Em cada momento,
                          somente pode haver no máximo uma única transação pendente para cada terminal.
                          Para minimizar cenários de desfazimento, é recomendável que a Automação Comercial confirme a
                          transação tão logo seja possível.Caso PTI_EFT_Exec retorne PTIRET_OK e a Automação Comercial
                          não confirmar a transação imediatamente, esta deve ser armazenada em memória não volátil
                          (arquivo) com todas as informações necessárias para confirmar ou desfazer a transação em caso de
                          queda de energia que ocorra após esse momento.
                          Eventos que podem levar a um desfazimento da transação são:
                          . Falha na impressora (quando a assinatura do portador do cartão for requerida);
                          . Mercadoria não pode ser entregue(mecanismo do dispensador falha ou equivalente);
                          . Falta de energia(portador do cartão utilizou um método de pagamento alternativo antes da volta
                            da energia).


        Entrada        :  pszTerminalId Identificador único do terminal(final nulo).

                          iStatus Status final da transação, conforme detalhado abaixo.

        Saida          :  Nenhuma.

        Retorno(piRet) :   PTIRET_OK Confirmação realizada.
                           PTIRET_INVPARAM Parâmetro inválido passado à função.

        Lista de possíveis status final para a transação:
                          =================================================

         Nome Valor      Descrição
                          =============    =====      =====================

         PTICNF_SUCCESS     1        Transação confirmada

         PTICNF_PRINTERR    2        Erro na impressora, desfazer a transação.
         PTICNF_DISPFAIL    3        Erro com o mecanismo dispensador, desfazer a transação.
         PTICNF_OTHERERR    4        Outro erro, desfazer a transação.
       */
        //============================================================================================================================
        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_EFT_Confirm(string uiTerminalId, int iResult, [MarshalAs(UnmanagedType.I2)] ref short piRet);


        //=========================================================================================================
        /*
            Function       :  PTI_EFT_Exec

            Descrição      :  Esta função efetua de fato a transação, utilizando os parâmetros que foram previamente
                              definidos através de PTI_EFT_AddParam.
                              Esta função é blocante, e somente retorna após a conclusão (ou falha) da transação.

            Entrada        :  pszTerminalId Identificador único do terminal (final nulo).

            Saida          :  Nenhuma.

            Retorno(piRet) :  PTIRET_OK Operação bem-sucedida (para venda, significa transação aprovada).
                              PTIRET_INVPARAM Parâmetro inválido passado à função
                              PTIRET_NOCONN O terminal está offline.
                              PTIRET_BUSY O terminal está ocupado processando outro comando
                              PTIRET_EFTERR A transação foi realizada, entretanto falhou
        */
        //=========================================================================================================
        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_EFT_Exec(string uiTerminalId, [MarshalAs(UnmanagedType.I2)] ref short piRet);


    //===========================================================================================================
     /*
        Function       :  PTI_EFT_GetInfo

        Descrição      :  A Automação Comercial deve chamar esta função iterativamente para recuperar
                          os dados relativos à transação que foi realizada(com ou sem sucesso) pelo terminal.
                         Esta função retorna imediatamente

           Entrada        :  pszTerminalId Identificador único do terminal (final nulo).


                             iInfo Identificador da informação a ser obtida,
                                            conforme o capítulo “TAG’s de entrada e saída”.


                             uiBufLen Tamanho (em bytes) do buffer referenciado pelo ponteiro pszValue

           Saida          :  pszValue Informação recuperada (final nulo).


           Retorno(piRet) :  PTIRET_OK Operação bem-sucedida, informação retornada

                             PTIRET_INVPARAM Parâmetro inválido passado à função.

                             PTIRET_BUFOVRFLW O tamanho do dado é maior que uiBufLen.
                             PTIRET_NOCONN O terminal está offline
                             PTIRET_BUSY        O terminal está ocupado processando outro comando
                             PTIRET_NODATA      Informação não disponível.
        */
        //===========================================================================================================        
        [DllImport("PTI_DLL.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_EFT_GetInfo(string uiTerminalId, int iInfo, int uiBufLen, StringBuilder pszValue, [MarshalAs(UnmanagedType.I2)] ref short piRet);


    //==============================================================================================================================
     /*
        Function       :  PTI_EFT_PrintReceipt

        Descrição      : Esta função faz com que o terminal imprima o comprovante da última transação realizada.
                         A Automação Comercial pode optar por:
                        . Utilizar esta função para imprimir uma ou ambas as vias (estabelecimento e/ou portador do
                          cartão) do comprovante de pagamento;
                        . Recuperar o conteúdo do comprovante através de PTI_EFT_GetInfo e:
                          .Imprimir uma ou ambas as vias em uma impressora dedicada
                          .Enviar a cópia do portador do cartão por e-mail ou outro tipo de mensageria;
            Nota: a via do estabelecimento deve sempre ser impressa quando PWINFO_CHOLDVERIF
            (recuperado através de PTI_EFT_GetInfo) indicar que a assinatura do portador do cartão é requerida.

        Entrada         : pszTerminalId Identificador único do terminal(final nulo).

                              iCopies Soma dos valores da tabela abaixo.

        Saidas          : pszValue Informação recuperada(final nulo).

            Retorno      :  PTIRET_OK Bem-sucedida, impressão iniciada
                            PTIRET_INVPARAM Parâmetro inválido passado à função.
                            PTIRET_NOCONN     O terminal está offline

                            PTIRET_BUSY O terminal está ocupado processando outro comando

                            PTIRET_NODATA Não há recibo a ser impresso
                            PTIRET_PRINTERR   Erro na impressora
                            PTIRET_NOPAPER    Impressora sem papel

                            Identificadores da cópia do recibo:
                              ===================================

                            Nome Valor     Descrição
                              ================      =====     ======================

                            PTIPRN_MERCHANT         1       Via do estabelecimento
                            PTIPRN_CHOLDER          2       Via do portador do cartão


        */
        //==============================================================================================================================
        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_EFT_PrintReceipt(string uiTerminalId, int iCopies, [MarshalAs(UnmanagedType.I2)] ref short piRet);



        //=======================================================================================================================
        /*
            Function       :  PTI_EFT_Start

            Descrição      :  A Automação Comercial deve chamar esta função para iniciar qualquer nova transação.
                              Esta função retorna imediatamente.

            Entrada        :  pszTerminalId Identificador único do terminal (final nulo).

                              iOper Tipo de transação, de acordo a tabela abaixo

            Saida          :  Nenhuma.

            Retorno        :  PTIRET_OK Operação bem-sucedida
                              PTIRET_INVPARAM   Parâmetro inválido passado à função.
                              PTIRET_NOCONN O terminal está offline
                              PTIRET_BUSY       O terminal está ocupado processando outro comando

                              Lista dos tipos de transações:
                              ==============================
                              Nome Valor     Descrição
                              ====================  =====     ====================================
                              PWOPER_SALE            33       Pagamento de mercadorias ou serviços.
                              PWOPER_ADMIN           32       Qualquer transação que não seja um pagamento (estorno,
                                                              pré-autorização, consulta, relatório, reimpressão de recibo, etc).
                              PWOPER_SALEVOID        34       Estorna uma transação de venda que foi previamente
                                                              realizada e confirmada.
        */
        //=======================================================================================================================
        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_EFT_Start(string uiTerminalId, int iOper, [MarshalAs(UnmanagedType.I2)] ref short piRet);


        //===============================================================================================
        /*             Function      :  PTI_End

             Descricao     :  Esta função deve ser a última função chamada pela Automação Comercial, quando finalizada
                              ou antes de descarregar a biblioteca de integração.
                              Neste momento, a biblioteca de integração libera todos recursos alocados (portas TCP, processos, memória, etc.).

             Input         :  none.

             Output        :  none.

             Return        :  none.
         */
        //===============================================================================================*/
        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_End();



    //=============================================================================================================================
     /*
          Function       :  PTI_ExecMenu

          Descrição      :  Esta função exibe o menu de opções que foi criado através de PTI_StartMenu
                            e PTI_AddMenuOption e identifica a seleção feita pelo usuário.
                            Esta função é blocante e somente retorna após a seleção de uma opção ou a ocorrência de um erro.

          Entrada        :  pszTerminalId Identificador único do terminal (final nulo).
                            pszPrompt Mensagem de texto com final nulo a ser apresentada ao usuário
                                           no topo do menu (máximo: 20 caracteres).
                                           Por exemplo: “SELECIONE UMA OPCAO:”.
                                           Caso NULL ou vazio, o menu é exibido a partir da primeira linha da tela.
                            uiTimeOutSec   Tempo máximo entre duas teclas pressionadas, em segundos.
                            puiSelection Índice (iniciado em zero) da opção que deve estar pré-selecionada quando o
                                         menu for inicialmente exibido, fazendo com que esta opção seja selecionada
                                           se o usuário simplesmente pressionar[OK]. Caso puiSelection não seja uma
                                          opção válida, nenhuma é pré-selecionada.


             Saida         :  puiSelection Índice(iniciado em zero) da opção que foi selecionada pelo usuário(somente
                                          se a função retornar PGWRET_OK)

              Retorno      :  PTIRET_OK Seleção do menu bem-sucedida.
                              PTIRET_INVPARAM Parâmetro inválido passado à função
                              PTIRET_NOCONN O terminal está offline.
                              PTIRET_BUSY O terminal está ocupado processando outro comando
                              PTIRET_TIMEOUT Nenhuma tecla foi pressionada durante o tempo especificado
                              PTIRET_CANCEL      Usuário pressionou a tecla[CANCELA].
       */
        //=============================================================================================================================
        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_ExecMenu(string terminalId, string pszPrompt, short uiTimeOutSec, ref short puiSelection, 
            [MarshalAs(UnmanagedType.I2)] ref short piRet);

        
    //===============================================================================================
    /*
      Function       :  PTI_GetData

          Descrição      :  Esta função realiza a captura de um único dado em um terminal previamente conectado.
                            Esta função é blocante e somente retorna após a captura de dado ser bem-sucedida ou falhar.

          Entrada        :  pszTerminalId Identificador único do terminal (final nulo).
                            pszPrompt Mensagem de texto com final nulo a ser apresentada ao usuário,
                                           descrevendo a informação a ser solicitada.
                                           Utilize ‘\r’ (código ASCII 13) para quebra de linha.Por exemplo: “VALOR DO SERVICO:”.
                            pszFormat Máscara de formatação com final nulo.Utilize ‘@’ (arroba) para as posições de caracteres editáveis.
                                           Por exemplo: “@@.@@@.@@@,@@” para um valor em centavos.
                                           Deve ser nulo (NULL) ou vazio para captura direta sem formatação
                            uiLenMin Número mínimo de caracteres
                            uiLenMax       Número máximo de caracteres
                            fFromLeft TRUE (1) para iniciar a digitação da esquerda;
                FALSE(0) para iniciar a digitação da direita.
        fAlpha         TRUE (1) para habilitar a entrada de caracteres não numéricos;
                FALSE(0) para permitir apenas caracteres numéricos.
               Nota: como a digitação de caracteres não numéricos em muitos terminais não é amigável,
                                           recomenda-se evitar o uso desse recurso sempre que possível
                            fMask          TRUE(1) para mascarar os caracteres digitados com asterisco
                                          (tipicamente, para digitação de senha); FALSE(0) para mostrar os caracteres digitados
                         uiTimeOutSec   Tempo máximo entre cada tecla pressionada, em segundos.
                         pszData Valor inicial para um dado a ser editado com final nulo.
                  uiCaptureLine  Índice da linha da tela (iniciando em 1) onde a informação digitada deve ser apresentada.
                                 Caso a legenda da mensagem também for apresentada nessa linha,
                                           a informação digitada será exibida logo após a legenda;
                                           senão, será exibida iniciando na primeira coluna.

          Saida          :  pszData Informação digitada com final nulo (somente caso a função retorne PTIRET_OK)

          Retorno        :   PTIRET_OK Captura de dado bem-sucedida
                             PTIRET_INVPARAM      Parâmetro inválido passado à função
                             PTIRET_NOCONN        O terminal está offline.
                             PTIRET_BUSY          O terminal está ocupado processando outro comando
                             PTIRET_TIMEOUT       Nenhuma tecla foi pressionada no tempo especificado.
                             PTIRET_CANCEL Usuário pressionou a tecla[CANCELA].

                             PTIRET_SECURITYERR A função foi rejeitada por questões de segurança.
        */
        //===============================================================================================        
        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_GetData(string uiTerminalId, string pszPrompt, string pszFormat, short uiLenMin,
            short uiLenMax, bool fFromLeft, bool fAlpha, bool fMask, short uiTimeOutSec, StringBuilder pszData, short uiCaptureLine, ref short piRet);



        //===============================================================================================*\
         /*

         Function     :  TPOSPGWLib.PTI_Init

         Descricao    : Esta função configura a biblioteca de integração e deve ser a primeira a ser chamada
                        pela Automação Comercial.A biblioteca de integração somente aceitará conexões do terminal
                        de pagamento após sua chamada.


         Entrada:       pszPOS_Company.........= Nome da empresa de Automação Comercial (final-nulo, até 40 caracteres e sem acentuação). Por exemplo, "KND SISTEMAS LTDA.".


                        pszPOS_Version.........= Nome e versão da aplicação de Automação Comercial(final-nulo, até 40 caracteres e sem acentuação).
                                                   Por exemplo, “SUPERVENDAS v1.01”.


                        pszPOS_Capabilities.....= Capacidades da Automação(soma dos valores abaixo) :
                                                   1:  funcionalidade de troco/saque;
                                                   2:  funcionalidade de desconto;
                                                   4:  valor fixo, sempre incluir;
                                                   8:  impressão das vias diferenciadas do comprovante para Cliente/Estabelecimento;
                                                   16: impressão do cupom reduzido.
                                                   32: utilização de saldo total do voucher para abatimento do valor da compra.

                        pszDataFolder...........= Caminho completo do diretório para armazenar dados e logs da biblioteca de integração.
                                                   Observação: O usuário do sistema operacional onde é executada a aplicação de Automação Comercial
                                                   deve ter permissão de gravação nesse diretório

                        uiTCP_Port  .............= Porta TCP à qual todos os terminais irão conectar.
                                                   Observação: esta porta deve estar habilitada para o recebimento de conexões
                                                   através de qualquer firewall que estiver no caminho entre a aplicação de Automação Comercial e o terminal de POS.

                        uiMaxTerminals......... = Número máximo de conexões simultâneas de terminais.

                        pszWaitMsg............. = Mensagem a ser apresentada na tela de qualquer terminal imediatamente após se conectar. Veja PTI_Display para informações de formatação.


                       uiAutoDiscSec........... = Tempo de ociosidade em segundos após o qual o terminal deve se desconectar da Automação Comercial
                                                   quando opera sem alimentação externa, ou zero para nunca desconectar.Veja PTI_Disconnect para informações adicionais.



         Saidas        :  none.

         Retorno       :  PTIRET_OK Operação bem-sucedida
                          PTIRET_INVPARAM    Parâmetro inválido informado à função
                          PTIRET_SOCKETERR   Erro ao iniciar a escuta da porta TCP informada
                          PTIRET_WRITEERR    Erro no uso do diretório informado
         */
         //===============================================================================================*/

        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_Init(string pszPOS_Company, string pszPOS_Version, string pszPOS_Capabilities, string pszDataFolder,
                int uiTCP_Port, int uiMaxTerminals, string pszWaitMsg, int uiAutoDiscSec, [MarshalAs(UnmanagedType.I2)] ref short piRet);
        

        //==========================================================================================================================
         /*
            Function       :  PTI_Print

            Descrição      :  Esta função imprime uma ou mais linhas de texto na impressora do terminal e
                              retorna imediatamente.Até 40 caracteres por linha podem ser impressos,
                              com quebras de linha identificadas pelo caractere ‘\r’ (código ASCII 13).
                              Caracteres além das 40 colunas serão descartados.
                              Um caractere de controle na primeira posição de uma linha indica a mudança
                              da fonte do caractere utilizada para o texto da linha inteira.
                              Caso o primeiro caractere de uma linha não é um caractere de controle,
                              a fonte padrão é utilizada.Os caracteres de controle suportados são:

                             Caractere de controle Código ASCII do caractere Efeito
                             =====================   =========================         ======
                                  ‘\v’                          11                Dobra a largura da fonte, consequentemente
                                                                                  o número de colunas suportado é dividido por dois.


                             "PTI_PrnFeed deve ser chamada após uma ou mais chamadas a PTI_Print."


            Entrada        :  pszTerminalId Identificador único do terminal (final nulo).

                              pszText Texto a ser impresso (final nulo).

            Saida          :  Nenhuma.

            Retorno(piRet) :  PTIRET_OK Operação bem-sucedida
                              PTIRET_INVPARAM      Parâmetro inválido passado à função
                              PTIRET_NOCONN        O terminal está offline
                              PTIRET_BUSY O terminal está ocupado processando outro comando
                              PTIRET_NOTSUPORTED Função não suportada pelo terminal
        */
       //==========================================================================================================================
        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_Print(string uiTerminalId, string text,
            [MarshalAs(UnmanagedType.I2)] ref short piRet);


        /*
        //========================================================================================================
              Function       :  PTI_PrnFeed

              Descrição      :  Esta função avança algumas linhas do papel da impressora,
                                para permitir que o usuário destaque o recibo

              Entrada        :  pszTerminalId  Identificador único do terminal (final nulo).

              Saida          :  Nenhuma.

              Retorno(piRet) :  PTIRET_OK            Operação bem-sucedida
                                PTIRET_INVPARAM      Parâmetro inválido passado à função
                                PTIRET_NOCONN        O terminal está offline
                                PTIRET_BUSY          O terminal está ocupado processando outro comando
                                PTIRET_PRINTERR      Erro na impressora
                                PTIRET_NOPAPER       Impressora sem papel
                                PTIRET_NOTSUPORTED   Função não suportada pelo terminal
         */
        //========================================================================================================
        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_PrnFeed(string uiTerminalId, [MarshalAs(UnmanagedType.I2)] ref short piRet);


        //====================================================================================================
        /*

            Funcao     :   PTI_PrnSymbolCode

            Descricao  :   Esta função imprime um código de barras ou QR code na impressora do terminal.

            Entradas   :   pszTerminalId  :  Identificador único do terminal (final nulo).
                           pszMsg         :  Código a ser impresso.
                           iSymbology     :  Tipo de código, conforme tabela abaixo.

            Saidas     :   Nenhuma.

            Retorno    :   PTIRET_OK          : Operação bem sucedida.
                           PTIRET_INVPARAM    : Parâmetro inválido passado à função.
                           PTIRET_INTERNALERR : Erro interno da biblioteca de integração.
                           PTIRET_NOCONN      : O terminal está offline.
                           PTIRET_BUSY        : O terminal está ocupado processando outro comando.

                           Tabela de tipos de código:
                           ==========================
                           Nome            Valor   Descrição
                           ============    =====   ===============
                                                   Código de barras padrão 128.
                           CODESYMB_128      2     Código de barras padrão 128. Pode-se utilizar aproximadamente
                                                   31 caracteres alfanuméricos.

                                                   Código de barras padrão ITF
                           CODESYMB_ITF      3     Pode-se utilizar aproximadamente
                                                   30 caracteres alfanuméricos.

                                                   QR Code. Com aceitação de
                           CODESYMB_QRCODE   4     aproximadamente 600 caracteres
                                                   alfanuméricos.
         */
        //====================================================================================================

        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_PrnSymbolCode(string uiTerminalId, string pszMsg, int iSymbology, [MarshalAs(UnmanagedType.I2)] ref short piRet);




        //=========================================================================================================
        /*
         Function       :  PTI_StartMenu

         Descrição      :  Esta função inicia a construção de um menu de opção para seleção pelo usuário.
                           Esta função retorna imediatamente.

         Entrada        :  pszTerminalId  Identificador único do terminal (final nulo).

         Saida          :  Nenhum.

         Retorno        :  PTIRET_OK       Criação do menu iniciada
                           PTIRET_NOCONN   O terminal está offline
                           PTIRET_BUSY     O terminal está ocupado processando outro comando.
        }
        */
        //=========================================================================================================
        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_StartMenu(string terminalId, [MarshalAs(UnmanagedType.I2)] ref short piRet);


        //============================================================================================================================================
       /*
        Function       :  PTI_WaitKey

        Descricao      :  Esta função aguarda o pressionar de uma tecla no terminal e apenas retorna após uma tecla ser
                          pressionada ou quando o tempo de espera se esgotar
                          Importante: Esta função somente deve ser utilizada para captura isolada de teclas, não devendo ser
                          sucessivamente chamada para captura de dados de entrada.Para este propósito, PTI_GetData deve
                          ser utilizado.


        Entrada        :  pszTerminalId Identificador único do terminal (final nulo).
                          uiTimeOutSec Tempo de espera do usuário, em segundos. Se igual a zero, a função retorna
                                         imediatamente, somente informando que uma tecla foi pressiona caso tenha
                                         sido feito antes da chamada à função. (Captura de tecla buferizada.)

        Saida          :  piKey Identificador da tecla que foi pressionada, de acordo com a tabela abaixo
                                (somente se o retorno da função for PTIRET_OK).

        Retorno        :  PTIRET_OK Operação bem-sucedida, uma tecla foi pressionada.
               PTIRET_NOCONN        O terminal está offline

               PTIRET_BUSY O terminal está ocupado processando outro comando.

               PTIRET_TIMEOUT Nenhuma tecla foi pressionada durante o período de tempo

                                    especificado.

       
        */
        //============================================================================================================================================
    
        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_WaitKey(string terminalId, short uiTimeOutSec,
            ref short piKey, [MarshalAs(UnmanagedType.I2)] ref short piRet);

        /////////////////////////////////////////////////////

        //========================================================================================================
        /*
            Function       :  PTI_ClearKey

            Descrição      :  Esta função limpa o buffer de teclas pressionadas,
                              para que a próxima chamada da função não considere qualquer tecla previamente pressionada.
                              Esta função retorna imediatamente.

            Entrada        :  pszTerminalId Identificador único do terminal (final nulo).

            Saida          :  Nenhuma.

            Retorno(piRet) :  PTIRET_OK Operação bem-sucedida
                              PTIRET_NOCONN         O terminal está offline
                              PTIRET_BUSY O terminal está ocupado processando outro comando.
        */
        //========================================================================================================
        [DllImport("PTI_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public extern static void PTI_ClearKey(string terminalId, short uiTimeOutSec,
            ref short piKey, [MarshalAs(UnmanagedType.I2)] ref short piRet);

////////////////////////////////////////////////////////////////////////



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
            PWINFO_RCPTFULL = 82, //Comprovante – via completa.
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

        //==========================================================================================
        // Definicoes das teclas do POS   @@@@@@@
        //==========================================================================================
        public enum PTIKEY : uint
        {
            PTIKEY_0 = 48,
            PTIKEY_1 = 49,
            PTIKEY_2 = 50,
            PTIKEY_3 = 51,
            PTIKEY_4 = 52,
            PTIKEY_5 = 53,
            PTIKEY_6 = 54,
            PTIKEY_7 = 55,
            PTIKEY_8 = 56,
            PTIKEY_9 = 57,
            PTIKEY_STAR = '*',
            PTIKEY_HASH = '#',
            PTIKEY_DOT = '.',
            PTIKEY_00 = 37,
            PTIKEY_BACKSP = 8,
            PTIKEY_OK = 13,
            PTIKEY_CANCEL = 27,
            PTIKEY_FUNC0 = 97,
            PTIKEY_FUNC1 = 98,
            PTIKEY_FUNC2 = 99,
            PTIKEY_FUNC3 = 100,
            PTIKEY_FUNC4 = 101,
            PTIKEY_FUNC5 = 102,
            PTIKEY_FUNC6 = 103,
            PTIKEY_FUNC7 = 104,
            PTIKEY_FUNC8 = 105,
            PTIKEY_FUNC9 = 106,
            PTIKEY_FUNC10 = 107,
            PTIKEY_TOUCH = 126,
            PTIKEY_ALPHA = 38

        }


        // Tipos de aviso sonoro:
        //==========================================================================================
        public enum PTIBEEP : uint
        {
            PTIBEEP_OK = 0,  // Sucesso
            PTIBEEP_WARNING = 1,  // Alerta
            PTIBEEP_ERROR = 2  // Erro

        }
        //==========================================================================================
        // Status do Terminal
        //==========================================================================================
        public enum PTISTAT : uint
        {
            PTISTAT_IDLE = 0,        // Terminal está on-line e aguardando por comandos.
            PTISTAT_BUSY = 1,        // Terminal está on-line, porém ocupado processando um comando
            PTISTAT_NOCONN = 2,      // Terminal está offline.
            PTISTAT_WAITRECON = 3   // Terminal está off-line. A transação continua sendo executada e
                                    // após sua finalização, o terminal tentará efetuar a reconexão
        }                           // automaticamente.
        #endregion

    }
}
