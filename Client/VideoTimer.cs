using System;
using System.Diagnostics;

public class VideoTimer
{
    private int totalFrames;
    private int fps;
    private int currentFrame;


    public VideoTimer(int totalFrames, int fps)
    {
        this.totalFrames = totalFrames;
        this.fps = fps;
        this.currentFrame = 1;
    }

    public void UpdateCurrentFrame(int currentFrame)
    {
        if (currentFrame >= 1 && currentFrame <= totalFrames)
        {
            this.currentFrame = currentFrame;
        }
    }
    public string GetCurrentTime()
    {
        int totalFramesElapsed = currentFrame - 1;
        int totalSecondsElapsed = totalFramesElapsed / fps;
        int minutes = totalSecondsElapsed / 60;
        int seconds = totalSecondsElapsed % 60;
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    public string FrameToTime(int frameNumber)
    {
        int totalFramesElapsed = frameNumber - 1;
        int totalSecondsElapsed = totalFramesElapsed / fps;

        int minutes = totalSecondsElapsed / 60;
        int seconds = totalSecondsElapsed % 60;

        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
