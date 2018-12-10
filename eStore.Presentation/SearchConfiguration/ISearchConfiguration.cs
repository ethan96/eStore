using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;

namespace eStore.Presentation.SearchConfiguration
{
    public interface ISearchConfiguration
    {
          eStore.Presentation.AJAX.AJAXFunctionType HeaderAJAXFunctionType { get;   }
          eStore.Presentation.AJAX.AJAXFunctionType AdvertisementAJAXFunctionType { get; }
          string ResultPageUrl { get;   }
    }
}
