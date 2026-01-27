namespace SisEUs.Domain.Comum.Validacao
{
    public class ValidadorDeDominio
    {
        private readonly List<string> _erros = [];

        public IReadOnlyCollection<string> Erros => _erros.AsReadOnly();
        public bool EhValido => _erros.Count == 0;

        public ValidadorDeDominio Validar(bool condicao, string mensagemErro)
        {
            if (!condicao)
                _erros.Add(mensagemErro);
            return this;
        }

        public ValidadorDeDominio ValidarNaoNuloOuVazio(string? valor, string nomeCampo)
        {
            if (string.IsNullOrWhiteSpace(valor))
                _erros.Add($"{nomeCampo} não pode ser nulo ou vazio.");
            return this;
        }

        public ValidadorDeDominio ValidarTamanho(string? valor, int min, int max, string nomeCampo)
        {
            if (valor != null && (valor.Length < min || valor.Length > max))
                _erros.Add($"{nomeCampo} deve ter entre {min} e {max} caracteres.");
            return this;
        }

        public ValidadorDeDominio ValidarEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                _erros.Add("Email não pode ser nulo ou vazio.");
                return this;
            }

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                if (addr.Address != email)
                    _erros.Add("Formato de email inválido.");
            }
            catch
            {
                _erros.Add("Formato de email inválido.");
            }
            return this;
        }

        public ValidadorDeDominio ValidarCpf(string? cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
            {
                _erros.Add("CPF não pode ser nulo ou vazio.");
                return this;
            }

            var cpfLimpo = cpf.Trim().Replace(".", "").Replace("-", "");

            if (cpfLimpo.Length != 11)
                _erros.Add("CPF deve ter 11 dígitos.");
            else if (!cpfLimpo.All(char.IsDigit))
                _erros.Add("CPF deve conter apenas números.");
            else if (cpfLimpo.Distinct().Count() == 1)
                _erros.Add("CPF inválido.");

            return this;
        }

        public ValidadorDeDominio ValidarSenha(string? senha, int tamanhoMinimo = 6)
        {
            if (string.IsNullOrWhiteSpace(senha))
            {
                _erros.Add("Senha não pode ser nula ou vazia.");
                return this;
            }

            if (senha.Length < tamanhoMinimo)
                _erros.Add($"Senha deve ter no mínimo {tamanhoMinimo} caracteres.");

            return this;
        }

        public void LancarSeInvalido()
        {
            if (!EhValido)
                throw new ExcecaoDeValidacaoMultipla(_erros);
        }

        public static ValidadorDeDominio Criar() => new();
    }
}
