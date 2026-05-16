using System;
using System.Collections.Generic;
using System.Text;

namespace Ftec.ProjetosWeb.Pagamento.Dominio.Enums
{
    public enum StatusPagamento
    {
        Pendente = 1,
        Processando = 2,
        Pago = 3,
        Recusado = 4,
        Cancelado = 5,
    }
}
