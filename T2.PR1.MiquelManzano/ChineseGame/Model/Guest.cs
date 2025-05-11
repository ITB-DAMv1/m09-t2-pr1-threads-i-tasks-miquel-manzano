
namespace ChineseGame.Model
{
    public enum State
    {
        eating,
        meditating,
        takingToothpick,
        leavingToothpick
    }

    public class Guest
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public State GuestState { get; set; }
        public int NumEats { get; set; }
        public TimeSpan HungerTime { get; set; }

        public Guest() { }

        // All methods change the GuestState.
        public void Think() { /* Return timeout 0.5 - 2 sec */ }
        public void Eat() { /* Return timeout 0.5 - 2 sec */ }
        public void RequestChopsticks() { /* Request both chopsticks, first the left one, then the right one */ }
        public void ReleaseChopsticks() { /* Release both chopsticks */ }
    }
}