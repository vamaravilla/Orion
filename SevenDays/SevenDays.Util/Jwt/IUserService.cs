namespace SevenDays.Util
{
    public interface IUserService
    {
        string GenerateToken(int idToken, int? idProfile);
    }

}
