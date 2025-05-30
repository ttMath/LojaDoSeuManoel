using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LojaDoSeuManoel.Domain.ValueObjects
{
    public record Produto(string ProdutoId, Dimensoes Dimensoes);
}
