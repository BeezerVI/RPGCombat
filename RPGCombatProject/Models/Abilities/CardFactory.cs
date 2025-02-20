
public static class CardFactory
{
    public static Card CreateCardFromDto(CardDto dto)
    {
        ITargetSelector selector = dto.TargetSelector == "All" 
            ? new AllTargetsSelector() 
            : new SingleTargetSelector();

        var effects = new List<ICardEffect>();
        foreach (var effectDto in dto.Effects)
        {
            switch (effectDto.Type)
            {
                case "DamageEffect":
                    effects.Add(new DamageEffect(effectDto.Value));
                    break;
                case "HealEffect":
                    effects.Add(new HealEffect(effectDto.Value));
                    break;
                case "ShieldEffect":
                    effects.Add(new ShieldEffect(effectDto.Value));
                    break;
                // Add additional cases as needed.
                default:
                    Console.WriteLine($"Unknown effect type: {effectDto.Type}");
                    break;
            }
        }
        return new Card(dto.Name, dto.Cost, selector, effects);
    }
}
