using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManagerAPI.Validaciones
{
    public class PesoArchivoValidacion : ValidationAttribute
    {
        private readonly int pesoMaximoMb;

        public PesoArchivoValidacion(int pesoMaximoMb)
        {
            this.pesoMaximoMb = pesoMaximoMb;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            IFormFile formFile = value as IFormFile;

            if (formFile == null)
                return ValidationResult.Success;

            if (formFile.Length > pesoMaximoMb * 1024 * 1024)
                return new ValidationResult($"El archivo enviado excedió el peso máximo permitido de {pesoMaximoMb} Mb.");

            return ValidationResult.Success;
        }
    }
}
