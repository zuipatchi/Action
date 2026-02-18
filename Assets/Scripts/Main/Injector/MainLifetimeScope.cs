using Main.Player;
using Main.Player.Action;
using Main.Player.Action.Dash;
using Main.Player.Action.Model;
using Main.Player.Action.Move;
using Main.Player.Animation;
using Main.Player.Hp.Model;
using Main.Player.Hp.ViewModel;
using VContainer;
using VContainer.Unity;

namespace Main.Injector
{
    public class MainLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<ActionBinder>().AsSelf();
            builder.RegisterComponentInHierarchy<PlayerAnimationPresenter>().AsSelf();
            builder.RegisterComponentInHierarchy<PlayerBody>().AsSelf();
            builder.Register<PlayerInputCallbacks>(Lifetime.Scoped).AsSelf();
            builder.Register<PlayerInputPresenter>(Lifetime.Scoped).AsSelf();
            builder.Register<PlayerControls>(Lifetime.Scoped).AsSelf();
            builder.Register<PlayerHpModel>(Lifetime.Scoped).AsSelf();
            builder.RegisterComponentInHierarchy<PlayerHpViewModel>().AsSelf();
            builder.RegisterComponentInHierarchy<PlayerMove>().AsSelf();
            builder.RegisterComponentInHierarchy<PlayerDash>().AsSelf();
            builder.Register<PlayerModel>(Lifetime.Scoped).AsSelf();
            builder.RegisterComponentInHierarchy<PlayerWeaponHitbox>().AsSelf();

            builder.Register<DashState>(Lifetime.Scoped).AsSelf();
            builder.RegisterComponentInHierarchy<DashAfterImageMesh>().AsSelf();
        }
    }
}
