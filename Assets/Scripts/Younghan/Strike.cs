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

    //���� ���������� �ǰ� ������ ���� ������ ����������
}
