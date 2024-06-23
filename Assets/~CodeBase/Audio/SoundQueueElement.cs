namespace CodeBase.Audio
{
    public sealed class SoundQueueElement
    {

        public Sound Sound { get; }
        public float BlendTIme { get; }
        public bool DoOverlap { get; }


        
        public SoundQueueElement(Sound sound, float blendTime = 0f, bool doOverlap = false)
        {
            Sound = sound;
            BlendTIme = blendTime;
            DoOverlap = doOverlap;
        }
    }
}