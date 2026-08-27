using System.ComponentModel.DataAnnotations;

namespace SIGSTO.Models
{
    public class Encadrant : Utilisateur
    {
        [MaxLength(200)]
        public string Departement { get; set; } = "";
    }
}
