using Microsoft.Xna.Framework;

namespace EvaFrontier.Lib
{
    public class WorldTime
    {

        #region Fields

        private double _elapsed = 0;

        #endregion

        #region Properties

        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public float Interval { get; set; } // Number of miliseconds to make an in-game minute

        public bool IsDay
        {
            get
            {
                return (Hour >= Sunrise && Hour < Sunset);
            }
        }

        public bool IsNight
        {
            get
            {
                return ((Hour >= 0 && Hour < Sunrise) || (Hour >= Sunset));
            }
        }

        public int Sunrise { get; set; }
        public int Sunset { get; set; }

        public float ambientLightLevel;
        public float spotlightLevel;

        #endregion

        #region Constructor

        public WorldTime() : this(1, 1, 1, 0, 0){}

        public WorldTime(int month, int day, int year, int hour, int minute)
        {
            Year = year;
            Month = month;
            Day = day;
            Hour = hour;
            Minute = minute;
            Interval = 50f;
            Sunrise = 6;
            Sunset = 18;
            ambientLightLevel = 1.0f;
            spotlightLevel = 0f;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return string.Format("Date: {0}/{1}/{2:0000} | Time: {3:00}:{4:00} | {5}", Month, Day, Year, Hour, Minute, IsDay ? "Day Time" : "Night Time");
        }

        public void Update(GameTime gameTime)
        {
            _elapsed += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_elapsed >= Interval)
            {
                _elapsed = 0;
                Minute++;
                
                if (Minute >= 60)
                {
                    Hour++;
                    Minute = 0;
                }

                if (Hour >= 24)
                {
                    Day++;
                    Hour = 0;
                }

                if (Day > 30)
                {
                    Month++;
                    Day = 1;
                }

                if (Month > 12)
                {
                    Year++;
                    Month = 1;
                }

                if (IsDay) {
                    ambientLightLevel -= (0.7f/12*60*Interval)*(float) _elapsed;
                    spotlightLevel += (1.0f / 12 * 60 * Interval) * (float)_elapsed;
                }

                if (IsNight) {
                    ambientLightLevel += (0.7f / 12 * 60 * Interval) * (float)_elapsed;
                    spotlightLevel -= (1.0f / 12 * 60 * Interval) * (float)_elapsed;
                }
            }
        }

        #endregion
    }
}
