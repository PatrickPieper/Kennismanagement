namespace La_Game.Dtos
{
    public class ParticipantDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName => string.Format("{0} {1}", FirstName, LastName);

    }
}