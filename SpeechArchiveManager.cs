# Super-Duper-Invention (SpeechArchiveAR)

A Unity mobile app for deaf communication with real-time speech archiving, statistical analysis, and AR integration. Designed to enhance communication accessibility through advanced speech-to-text archival systems.

## Features

- Speech archiving with file rotation
- Real-time transcription and display
- Statistical analysis of conversations
- Augmented reality interfaces
- Cross-platform (Android/iOS)

## Setup

1. Clone this repository.
2. Open the project in Unity (2022.3 LTS or later).
3. Ensure Android SDK and JDK are installed for Android builds.

## Building the APK

1. Open the project in Unity.
2. Go to `File -> Build Settings`.
3. Select `Android` as the platform and switch if necessary.
4. Click `Build` and choose a location for the APK.

## Usage

- The app will create an archive directory at `Application.persistentDataPath/SpeechArchive`.
- Use the `TestSaveFunction` to verify the archive system.
- Check the UI for archive statistics and paths.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
