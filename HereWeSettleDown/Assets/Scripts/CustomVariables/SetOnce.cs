namespace CustomVariables
{
    public class SetOnce<T>
    {
        private readonly object threadLock = new object();
        private bool setted;
        private T value;

        public SetOnce() { }

        public SetOnce(T value)
        {
            this.value = value;
        }

        public T Value
        {
            get
            {
                lock (threadLock)
                {
                    if (!setted) 
                        return default;
                    return value;
                }
            }
            set
            {
                lock (threadLock)
                {
                    if (!setted)
                    {
                        setted = true;
                        this.value = value;
                    }
                }
            }
        }

        public static implicit operator T(SetOnce<T> toConvert)
        {
            return toConvert.value;
        }
    }
}

