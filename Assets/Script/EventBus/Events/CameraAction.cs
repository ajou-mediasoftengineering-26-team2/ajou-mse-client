public readonly struct CameraAction
{
    public readonly CameraType ActionCode;
    public CameraAction(CameraType cameraType) => ActionCode = cameraType;
}
