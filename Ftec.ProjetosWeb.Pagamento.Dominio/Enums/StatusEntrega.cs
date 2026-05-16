using System;
using System.Collections.Generic;
using System.Text;

namespace Ftec.ProjetosWeb.Pagamento.Dominio.Enums
{
    public enum StatusEntrega
    {
        Pendente = 1,
        Preparando = 2,
        Enviado = 3,
        EmTransito = 4,
        Entregue = 5,
        Cancelado = 6
    }
}
