# super-duper-invention
Unity-based Android/iOS application for deaf communication featuring speech archiving, real-time transcription, and augmented reality interfaces
## 📝 **README.md CONTENT - READY TO PASTE:**

```markdown
# Super-Duper-Invention 🚀

**Unity mobile app for deaf communication with real-time speech archiving and statistical analysis**

## 🎯 Project Overview

A Unity-based Android/iOS application designed to enhance communication accessibility for the deaf community through advanced speech-to-text archival systems.

## ✨ Features

- **🗣️ Speech Archiving**: Real-time conversation recording and storage
- **📊 Statistical Analysis**: Track conversation patterns and word frequency  
- **📱 Mobile Optimized**: Cross-platform (Android/iOS) with battery awareness
- **🔄 File Rotation**: Automatic cleanup to prevent storage bloat
- **💾 Backup System**: Memory buffer for failed saves with retry logic
- **📈 Real-time UI**: Live updates and archive statistics

## 🚀 Quick Start

### Prerequisites
- Unity 2022.3 LTS or newer
- Android/iOS build support
- Git for version control

### Installation
1. **Clone repository**
   ```bash
   git clone https://github.com/smedum/super-duper-invention.git
   ```

2. **Open in Unity**
   - Open Unity Hub
   - Add project from cloned folder
   - Open project

3. **Setup Scene**
   - Attach `SpeechArchiveManager` script to a GameObject
   - Assign UI Text element to `displayText` field
   - Configure archive settings in Inspector

4. **Build & Deploy**
   - File → Build Settings → Android/iOS
   - Development Build ✅
   - Build APK

## 📁 Project Structure

```
Assets/
└── Scripts/
    └── SpeechArchiveManager.cs     # Main archive management system
        - SaveRecording()           # Primary save method
        - GetArchiveStats()         # Statistics retrieval
        - TestSaveFunction()        # Debug/testing method
```

## 🛠️ Core Components

### SpeechArchiveManager
The main script handling all archive operations:

```csharp
// Basic usage
SaveRecording(audioData, "conversation_1.dat");
string stats = GetArchiveStats();
```

**Key Settings:**
- `maxArchivedFiles`: File count limit (default: 100)
- `maxFileSizeMB`: Individual file size limit (default: 10MB)
- `enableFileRotation`: Automatic file cleanup (default: true)

## 📊 API Reference

### Public Methods
- `SaveRecording(byte[] audioData, string filename)` - Archive speech data
- `GetArchiveStats()` - Returns file count and storage usage  
- `TestSaveFunction()` - Creates test file for validation
- `FlushBackupBuffer()` - Retry failed saves from memory buffer

## 🎮 Usage Example

1. **Initialize**: App starts → archive path displays
2. **Record**: Speech detected → automatic archiving
3. **Monitor**: Real-time statistics update
4. **Manage**: Automatic file rotation maintains performance

## 🤝 Contributing

We welcome contributions! Please see our contributing guidelines for details.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Built for the deaf community
- Designed with accessibility as priority
- Open source for collaborative improvement

---

al documentation** for contributors
- ✅ **Accessibility focus** aligns with project goals

