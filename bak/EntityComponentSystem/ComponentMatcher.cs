namespace Alien_World.Entity_Component_System
{
    using ComponentMask = BitVector64;

    public static class ComponentMatcher
    {
        public static ComponentMask Get<C1>()
        {
            ComponentMask mask = new ComponentMask();
            mask[1 << ComponentIndexer.GetFamily<C1>()] = true;
            return mask;
        }

        public static ComponentMask Get<C1, C2>()
        {
            ComponentMask mask = new ComponentMask();
            mask[1 << ComponentIndexer.GetFamily<C1>()] = true;
            mask[1 << ComponentIndexer.GetFamily<C2>()] = true;
            return mask;
        }

        public static ComponentMask Get<C1, C2, C3>()
        {
            ComponentMask mask = new ComponentMask();
            mask[1 << ComponentIndexer.GetFamily<C1>()] = true;
            mask[1 << ComponentIndexer.GetFamily<C2>()] = true;
            mask[1 << ComponentIndexer.GetFamily<C3>()] = true;
            return mask;
        }

        public static ComponentMask Get<C1, C2, C3, C4>()
        {
            ComponentMask mask = new ComponentMask();
            mask[1 << ComponentIndexer.GetFamily<C1>()] = true;
            mask[1 << ComponentIndexer.GetFamily<C2>()] = true;
            mask[1 << ComponentIndexer.GetFamily<C3>()] = true;
            mask[1 << ComponentIndexer.GetFamily<C4>()] = true;
            return mask;
        }

        public static ComponentMask Get<C1, C2, C3, C4, C5>()
        {
            ComponentMask mask = new ComponentMask();
            mask[1 << ComponentIndexer.GetFamily<C1>()] = true;
            mask[1 << ComponentIndexer.GetFamily<C2>()] = true;
            mask[1 << ComponentIndexer.GetFamily<C3>()] = true;
            mask[1 << ComponentIndexer.GetFamily<C4>()] = true;
            mask[1 << ComponentIndexer.GetFamily<C5>()] = true;
            return mask;
        }

        public static ComponentMask Get<C1, C2, C3, C4, C5, C6>()
        {
            ComponentMask mask = new ComponentMask();
            mask[1 << ComponentIndexer.GetFamily<C1>()] = true;
            mask[1 << ComponentIndexer.GetFamily<C2>()] = true;
            mask[1 << ComponentIndexer.GetFamily<C3>()] = true;
            mask[1 << ComponentIndexer.GetFamily<C4>()] = true;
            mask[1 << ComponentIndexer.GetFamily<C5>()] = true;
            mask[1 << ComponentIndexer.GetFamily<C6>()] = true;
            return mask;
        }

        public static ComponentMask Get<C1, C2, C3, C4, C5, C6, C7>()
        {
            ComponentMask mask = new ComponentMask();
            mask[1 << ComponentIndexer.GetFamily<C1>()] = true;
            mask[1 << ComponentIndexer.GetFamily<C2>()] = true;
            mask[1 << ComponentIndexer.GetFamily<C3>()] = true;
            mask[1 << ComponentIndexer.GetFamily<C4>()] = true;
            mask[1 << ComponentIndexer.GetFamily<C5>()] = true;
            mask[1 << ComponentIndexer.GetFamily<C6>()] = true;
            mask[1 << ComponentIndexer.GetFamily<C7>()] = true;
            return mask;
        }
    }
}
