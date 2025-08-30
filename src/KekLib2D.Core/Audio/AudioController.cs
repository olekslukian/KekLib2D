using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace KekLib2D.Core.Audio;

public class AudioController : IDisposable
{
    private readonly List<SoundEffectInstance> _activeSfxInstances;
    private float _previousSongVolume;
    private float _previousSfxVolume;
    public bool IsMuted { get; private set; }
    public float SongVolume
    {
        get
        {
            if (IsMuted)
            {
                return 0.0f;
            }

            return MediaPlayer.Volume;
        }

        set
        {
            if (IsMuted)
            {
                return;
            }

            MediaPlayer.Volume = Math.Clamp(value, 0.0f, 1.0f);
        }
    }
    public float SfxVolume
    {
        get
        {
            if (IsMuted)
            {
                return 0.0f;
            }

            return SoundEffect.MasterVolume;
        }

        set
        {
            if (IsMuted)
            {
                return;
            }

            SoundEffect.MasterVolume = Math.Clamp(value, 0.0f, 1.0f);
        }
    }
    public bool IsDisposed { get; private set; }

    public AudioController()
    {
        _activeSfxInstances = [];
    }

    ~AudioController() => Dispose();

    public void Update()
    {
        for (int i = _activeSfxInstances.Count - 1; i >= 0; i--)
        {
            SoundEffectInstance instance = _activeSfxInstances[i];

            if (instance.State == SoundState.Stopped)
            {
                if (!instance.IsDisposed)
                {
                    instance.Dispose();
                }

                _activeSfxInstances.RemoveAt(i);
            }
        }
    }

    public SoundEffectInstance PlaySoundEffect(SoundEffect soundEffect)
    {
        return PlaySoundEffect(soundEffect, 1.0f, 1.0f, 0.0f, false);
    }

    public SoundEffectInstance PlaySoundEffect(SoundEffect soundEffect, float volume, float pitch, float pan, bool isLooped)
    {
        SoundEffectInstance instance = soundEffect.CreateInstance();

        instance.Volume = volume;
        instance.Pitch = pitch;
        instance.Pan = pan;
        instance.IsLooped = isLooped;

        instance.Play();

        _activeSfxInstances.Add(instance);

        return instance;
    }

    public void PlaySong(Song song, bool isRepeating = true)
    {
        if (MediaPlayer.State == MediaState.Playing)
        {
            MediaPlayer.Stop();
        }

        MediaPlayer.Play(song);
        MediaPlayer.IsRepeating = isRepeating;
    }

    public void PauseAudio()
    {
        MediaPlayer.Pause();

        foreach (SoundEffectInstance instance in _activeSfxInstances)
        {
            if (instance.State == SoundState.Playing)
            {
                instance.Pause();
            }
        }
    }

    public void ResumeAudio()
    {
        MediaPlayer.Resume();

        foreach (SoundEffectInstance instance in _activeSfxInstances)
        {
            instance.Resume();
        }
    }

    public void MuteAudio()
    {
        _previousSongVolume = MediaPlayer.Volume;
        _previousSfxVolume = SoundEffect.MasterVolume;

        MediaPlayer.Volume = 0.0f;
        SoundEffect.MasterVolume = 0.0f;

        IsMuted = true;
    }

    public void UnmuteAudio()
    {
        MediaPlayer.Volume = _previousSongVolume;
        SoundEffect.MasterVolume = _previousSfxVolume;

        IsMuted = false;
    }

    public void ToggleMute()
    {
        if (IsMuted)
        {
            UnmuteAudio();
        }
        else
        {
            MuteAudio();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (IsDisposed)
        {
            return;
        }

        if (disposing)
        {
            foreach (SoundEffectInstance instance in _activeSfxInstances)
            {
                instance.Dispose();
            }

            _activeSfxInstances.Clear();
        }

        IsDisposed = true;
    }
}
