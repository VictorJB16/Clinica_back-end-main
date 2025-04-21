namespace Clinica_back_end.Entities
{
    public class Sucursal
    {
        public int SucursalId { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }

        public ICollection<Cita> Citas { get; set; }
    }

}
