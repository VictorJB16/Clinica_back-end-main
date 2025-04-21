namespace Clinica_back_end.Entities
{
    public class Cita
    {
        public int CitaId { get; set; }
        public DateTime FechaHora { get; set; }
        public string Lugar { get; set; }
        public string Status { get; set; } // ACTIVA, CANCELADA

        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; }

        public int SucursalId { get; set; }
        public Sucursal Sucursal { get; set; }

        public int TipoCitaId { get; set; }
        public TipoCita TipoCita { get; set; }
    }

}
