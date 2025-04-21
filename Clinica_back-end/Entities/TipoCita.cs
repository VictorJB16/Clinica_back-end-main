namespace Clinica_back_end.Entities
{
    public class TipoCita
    {
        public int TipoCitaId { get; set; }
        public string Nombre { get; set; } // Medicina General, Odontología, etc.

        public ICollection<Cita> Citas { get; set; }
    }

}
