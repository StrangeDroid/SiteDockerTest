window.playSound = function (audioId) {
    const audio = document.getElementById(audioId);
    if (audio) {
        audio.play().catch(e => console.warn("Audio play error:", e));
    }
}
window.initAudio = function () {
    const bg = document.getElementById("backgroundMusic");
    if (bg) {
        bg.muted = false;
        bg.play().catch(err => {
            console.warn("Background music play failed:", err);
        });
    }

    // Preload sound effects to satisfy browser policies
    const sounds = ["successSound", "loseSound", "defaultSound"];
    for (const id of sounds) {
        const audio = document.getElementById(id);
        if (audio) {
            audio.play().catch(() => { });
            audio.pause();
            audio.currentTime = 0;
        }
    }
};
window.setMusicVolume = function (vol) {
    const bg = document.getElementById("backgroundMusic");
    if (bg) bg.volume = vol;
};

window.setEffectsVolume = function (vol) {
    ["successSound", "loseSound", "defaultSound"].forEach(id => {
        const s = document.getElementById(id);
        if (s) s.volume = vol;
    });
};

window.saveVolumePrefs = function (musicVol, effectsVol) {
    document.cookie = `musicVolume=${musicVol}; path=/; max-age=2592000`;  // 30 days
    document.cookie = `effectsVolume=${effectsVol}; path=/; max-age=2592000`;
};
