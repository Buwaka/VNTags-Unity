/// <summary>
///     Defines a contract for components that can manage visual transitions and their active state.
///     Implementations of this interface are responsible for handling the visual effects of
///     transitioning elements, such as fading in or out, and enabling/disabling their rendering.
/// </summary>
public interface IVNTransitionable
{
    /// <summary>
    ///     Checks if the component is currently in the process of a visual transition.
    /// </summary>
    /// <returns>True if a transition is active, false otherwise.</returns>
    public bool IsTransitioning();

    /// <summary>
    ///     Initiates the transition process. This method should be called when a transition
    ///     begins, allowing the implementing component to set up its initial state for the transition.
    /// </summary>
    public void Start();

    /// <summary>
    ///     Applies a fade-in effect to the component based on the provided progress.
    ///     The progress value typically ranges from 0.0 (fully transparent) to 1.0 (fully opaque).
    /// </summary>
    /// <param name="progress">A float value representing the current progress of the fade-in effect (e.g., 0.0 to 1.0).</param>
    public void FadeIn(float progress);

    /// <summary>
    ///     Applies a fade-out effect to the component based on the provided progress.
    ///     The progress value typically ranges from 0.0 (fully opaque) to 1.0 (fully transparent).
    /// </summary>
    /// <param name="progress">A float value representing the current progress of the fade-out effect (e.g., 0.0 to 1.0).</param>
    public void FadeOut(float progress);

    /// <summary>
    ///     Concludes the transition process. This method should be called when a transition
    ///     finishes, allowing the implementing component to finalize its state (e.g., reset flags, ensure full
    ///     visibility/invisibility).
    /// </summary>
    public void Finish();

    /// <summary>
    ///     Enables the component, making it visible or active within the scene.
    /// </summary>
    public void Enable();

    /// <summary>
    ///     Disables the component, making it invisible or inactive within the scene.
    /// </summary>
    public void Disable();
}