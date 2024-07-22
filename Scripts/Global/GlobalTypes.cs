namespace GlobalTypes
{
    public enum ColorSet { red, blue, white, brown, green, yellow, purple }; // Global color set
    public enum ElementState { none, normal, water, fire, air, earth };
    //public enum WorldID { PurpleForest, DistantShoreline }
    //public enum InteractableType { teleport, checkpoint, sign, entrance }; // checkpoint is also a teleport that brings you back
    //public enum Direction { left, right, up, down };

    public enum ScreenTransition { restart, hubEntry, levelEntry, hubEntryCompleted, worldEntry, optionsEntry, creditsEntry, menuEntry, gameEntry };
}
