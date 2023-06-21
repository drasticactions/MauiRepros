using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkupError
{
    public class StringResourceXamlMarkupExtension : IMarkupExtension<string>
    {
        private string test;

        public string Test {
            get => test;
            set => test = value;
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return (this as IMarkupExtension<string>).ProvideValue(serviceProvider);
        }

        public string ProvideValue(IServiceProvider serviceProvider)
        {
            return "test";
        }
    }
}
