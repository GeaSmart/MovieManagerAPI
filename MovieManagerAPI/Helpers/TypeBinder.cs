using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManagerAPI.Helpers
{
    public class TypeBinder<T> : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var nombrePropiedad = bindingContext.ModelName;
            var proveedor = bindingContext.ValueProvider.GetValue(nombrePropiedad);

            if (proveedor == ValueProviderResult.None)
                return Task.CompletedTask;

            try
            {
                var valor = JsonConvert.DeserializeObject<T>(proveedor.FirstValue);
                bindingContext.Result = ModelBindingResult.Success(valor);
            }
            catch (Exception)
            {
                bindingContext.ModelState.TryAddModelError(nombrePropiedad, $"Valor inválido para tipo {typeof(T)}.");
            }
            return Task.CompletedTask;
        }
    }
}
