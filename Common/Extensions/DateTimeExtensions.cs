namespace HeroesCup.Web.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static bool IsExpired(this long endDate)
        {
            var today = DateTime.Now.Date;
            bool expiredMission = today > endDate.ConvertToLocalDateTime().Date;

            return expiredMission;
        }
    }
}