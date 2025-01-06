public struct Strike
{
    public int power {
        private set;
        get;
    }

    public byte extension {
        private set;
        get;
    }

    public Spell[] spells {
        private set;
        get;
    }

    public Strike(int power, byte extension, Spell[] spells)
    {
        this.power = power;
        this.extension = extension;
        this.spells = spells;
    }

    //예전 던젼나이츠 피격 판정에 대한 내용을 정리해주자
}
