using System;
using System.Globalization;
using UnityEngine;

namespace VNTags.Tags
{
    /// <param name="overrideSeconds">
    ///     Hard-set wait time from this tag's optional parameter, or null to derive it from the line's
    ///     dialogue length instead.
    /// </param>
    public delegate bool WaitHandler(VNTagContext context, float? overrideSeconds);

    /// <summary>
    ///     Pauses the line without requiring player input, for a duration decided by its handler (see
    ///     <see cref="VNTagEventAnnouncer.onWaitTag" />) — by default derived from how long the line's
    ///     dialogue is, so a short reaction and a long paragraph don't get the same pause. An optional
    ///     first parameter hard-sets the wait instead, for a line that needs more (or less) time than its
    ///     length alone would suggest.
    ///
    ///     Auto-appended to every dialogue line played through a flow-graph node in place of Confirm (see
    ///     <see cref="VNTagDeserializer.Parse" />'s <c>autoPaceLines</c>), since those shouldn't wait on
    ///     the player — write <c>{Wait;seconds}</c> yourself only to override a specific line; the
    ///     auto-appended one is skipped when you already have.
    /// </summary>
    public class WaitTag : VNTag
    {
        private float? _overrideSeconds;

        public float? OverrideSeconds => _overrideSeconds;

        public override string GetTagName()
        {
            return "Wait";
        }

        public override bool Deserialize(VNTagDeserializationContext context, params string[] parameters)
        {
            if (parameters != null && parameters.Length > 0 && !string.IsNullOrEmpty(parameters[0]))
            {
                if (float.TryParse(parameters[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float seconds))
                {
                    _overrideSeconds = seconds;
                }
                else
                {
                    Debug.LogWarning($"WaitTag: Deserialize: failed to parse seconds '{parameters[0]}', "
                                   + $"falling back to the length-based default. {context}");
                }
            }

            return true;
        }

        public override string Serialize(VNTagSerializationContext context)
        {
            return _overrideSeconds.HasValue
                ? SerializeHelper(GetTagName(), _overrideSeconds.Value)
                : SerializeHelper(GetTagName());
        }

        protected override VNTagParameters Parameters(VNTagParameters currentParameters)
        {
            var secondsParameter = new VNTagParameter(1,
                                                       "Seconds",
                                                       TypeCode.Single,
                                                       "Hard-sets the wait in seconds. Leave empty to use the "
                                                     + "line's dialogue length instead.",
                                                       true);

            currentParameters.DefaultParameter(secondsParameter,
                                               _overrideSeconds?.ToString(CultureInfo.InvariantCulture) ?? "");

            return currentParameters;
        }

        protected override void Execute(VNTagContext context, out bool isFinished)
        {
            isFinished = ExecuteHelper(VNTagEventAnnouncer.onWaitTag?.Invoke(context, _overrideSeconds));
        }
    }
}
