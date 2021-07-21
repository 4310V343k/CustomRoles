using CustomRoles.API;
using Exiled.Events.EventArgs;
using UnityEngine;

namespace CustomRoles.Roles
{
    public class BallisticZombie : CustomRole
    {
        public override RoleType Type { get; set; } = Plugin.Singleton.Config.RoleConfigs.BallisticCfg.RoleType;
        public override int MaxHealth { get; set; } = Plugin.Singleton.Config.RoleConfigs.BallisticCfg.MaxHealth;
        public override string Name { get; set; } = Plugin.Singleton.Config.RoleConfigs.BallisticCfg.Name;
        protected override string Description { get; set; } =
            "A regular zombie that'll explode when killed.";

        protected override void LoadEvents()
        {
            Exiled.Events.Handlers.Player.Dying += OnDying;
        }

        private void OnDying(DyingEventArgs ev)
        {
            if (ev.Target == Player)
            {
                Plugin.Singleton.Methods.Spawn(ev.Target.Position, Vector3.zero, 0.5f, ItemType.GrenadeFrag, ev.Target);
            }
        }
    }
}