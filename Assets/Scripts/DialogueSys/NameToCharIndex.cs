public static class NameToCharIndex
{
    public static int GetCharacterIndexFromName(string name)
    {
        int characterIndex = -1;

        switch (name)
        {
            case "TEMPERANCE":
                characterIndex = GameMaster.temperanceIndex;
                break;

            case "CAPRICORN":
                characterIndex = GameMaster.capricornIndex;
                break;

            case "CANCER":
                characterIndex = GameMaster.cancerIndex;
                break;

            default:
                break;
        }

        return characterIndex;
    }

    public static void SetCharacterIndexFromName(string name, int newIndex)
    {
        switch (name)
        {
            case "TEMPERANCE":
                GameMaster.temperanceIndex = newIndex;
                break;

            case "CAPRICORN":
                GameMaster.capricornIndex = newIndex;
                break;

            case "CANCER":
                GameMaster.cancerIndex = newIndex;
                break;

            default:
                break;
        }
    }
}
