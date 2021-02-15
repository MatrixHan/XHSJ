
[System.Serializable]
public enum WorldUnit {
    /// <summary>
    /// ��
    /// </summary>
    None = 0,
    /// <summary>
    /// �ϰ�
    /// </summary>
    Impede = 1 << 0,
    /// <summary>
    /// Ұ��
    /// </summary>
    Monster = 1 << 1,
    /// <summary>
    /// ����
    /// </summary>
    City = 1 << 2,
    /// <summary>
    /// �����޷���Խ�ĸ�ɽ
    /// </summary>
    Mountain = 1 << 3,
    /// <summary>
    /// ���ִ�
    /// </summary>
    NewVillage = 1 << 4,



    /// <summary>
    /// �ȼ�����1
    /// </summary>
    Level1 = 1 << 10,
    /// <summary>
    /// �ȼ�����2
    /// </summary>
    Level2 = 1 << 11,
    /// <summary>
    /// �ȼ�����3
    /// </summary>
    Level3 = 1 << 12,
    /// <summary>
    /// �ȼ�����4
    /// </summary>
    Level4 = 1 << 13,
    /// <summary>
    /// �ȼ�����5
    /// </summary>
    Level5 = 1 << 14,
}
