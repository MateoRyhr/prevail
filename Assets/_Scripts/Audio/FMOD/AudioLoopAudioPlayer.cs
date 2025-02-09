using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioLoopAudioPlayer : MonoBehaviour
{
    [SerializeField] private Transform _audioTransform;
    [SerializeField] private float _volume;
    private EventInstance _loopAudio;

    void Update() {
        if (_loopAudio.isValid()) {
            var attributes = RuntimeUtils.To3DAttributes(_audioTransform.position);
            _loopAudio.set3DAttributes(attributes);
        }
    }

    public void PlayLoopAudio(Sound sound)
    {
        // if(_loopAudio)
        if(!_loopAudio.isValid())
            _loopAudio = RuntimeManager.CreateInstance(sound.soundEvent);
        _loopAudio.setVolume(_volume);
        _loopAudio.start();
    }

    public void StopLoopAudio()
    {
        _loopAudio.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _loopAudio.release();
    }
}
