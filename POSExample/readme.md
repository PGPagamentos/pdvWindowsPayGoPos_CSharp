# Exemplo de integração em C# Windows com a biblioteca PTI_DLL da plataforma de transações com cartão PayGo Web

### Funcionalidades implementadas neste exemplo:
 
  - Inicialização - Configura Biblioteca de Integração.
  - Conexão com POS.
  - Identificação do terminal conectado.
  - Menu.
  - Captura de Dados com e sem mascara.
  - Emissão de QR Code e Codigo Barras.
  - Fluxo de Venda Completo.
  - Cancelamento
  
### Pré-requisitos
  - Visual Studio 2017 ou posterior 
  - Windows
  - Cadastro no ambiente de testes/sandbox do PayGo Web
    - código do Ponto de Captura (PdC)
    - POS


## Ambiente e configuração da aplicação

* O arquivo da *PTI_DLL.dll* (incluso no repositório) deve ser incluído na pasta raíz do projeto;
* A pasta do projeto deve possuir permissão de leitura e escrita;

## OBS
Ao executar a aplicação, será utilizado o mesmo diretório do projeto para salvar os arquivos relativos à comunição da Automação com a infraestrutura e os 
Logs da execução da Pay&Go ficarão na pasta *PayGoPOS*.




