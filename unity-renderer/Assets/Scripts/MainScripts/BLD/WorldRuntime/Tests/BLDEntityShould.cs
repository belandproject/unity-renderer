using BLD.Components;
using BLD.Models;
using NSubstitute;
using NUnit.Framework;

public class BLDEntityShould
{
    [Test]
    public void CleanUpEntityComponents()
    {
        BelandEntity entity = new BelandEntity();
        IEntityComponent component = Substitute.For<IEntityComponent>();
        entity.components.Add(CLASS_ID_COMPONENT.NONE, component);

        entity.Cleanup();

        component.Received().Cleanup();
    }
}