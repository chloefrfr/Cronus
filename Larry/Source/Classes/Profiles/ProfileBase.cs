using Larry.Source.Interfaces;

namespace Larry.Source.Classes.Profiles
{
    public abstract class ProfileBase
    {
        protected IProfile Profile { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileBase"/> class.
        /// </summary>
        /// <param name="profile">The profile instance.</param>
        protected ProfileBase(IProfile profile)
        {
            Profile = profile;
        }

        /// <summary>
        /// Gets the profile instance.
        /// </summary>
        /// <returns>The profile instance.</returns>
        public abstract IProfile GetProfile();
    }
}
