# speakit


프로젝트 개요: SpeakIT

SpeakIT은 사용자의 언어 학습 경험을 향상시키기 위해 설계된 영어 학습 애플리케이션입니다. 
이 앱은 사용자가 듣고 말하는 과정을 통해 영어를 학습할 수 있도록 설계되었으며, Google의 TTS(Text-to-Speech) 및 STT(Speech-to-Text) 기술을 활용하여 음성 인식 및 재생 기능을 제공합니다. 

스크립트 파일 요약

1. Manager 폴더
 - PlayerPrefsManager.cs: 사용자 설정 및 게임 데이터를 로컬 저장소에 저장하고 불러오는 기능을 관리합니다.
 - STTManager.cs: Google STT 기능을 통합하여 사용자가 말한 내용을 텍스트로 변환하는 기능을 관리합니다.
 - DeepLinkManager.cs: 앱 내에서 딥 링크를 처리하여 특정 화면으로 바로 이동할 수 있도록 관리합니다.
 - UIManager.cs: UI 요소들의 전반적인 상태를 관리하고, 화면 전환 및 팝업을 처리합니다.
 - TTSManager.cs: Google TTS 기능을 사용하여 텍스트를 음성으로 변환하고 이를 재생하는 기능을 제공합니다.
 - GameManager.cs: 전체 게임의 상태와 흐름을 관리하는 메인 매니저 클래스입니다.
 - OpenBrowser.cs: 외부 브라우저를 열어 웹 페이지를 표시하는 기능을 제공합니다.
 - NetworkManager.cs: 네트워크 연결 상태를 관리하고, 서버와의 통신을 처리합니다.

2. UI 폴더
 - UICanvasMatchController.cs: UI 캔버스에서 매칭 관련 기능을 관리하는 스크립트입니다.
 - DragExit.cs: 드래그 동작에서 UI 요소를 특정 위치에서 벗어나게 하는 기능을 제공합니다.
 - UIBottomBar.cs: 하단 바의 UI 요소들을 제어하고 상태를 업데이트합니다.
 - UIPopup_Tweener.cs: 팝업 애니메이션을 관리하여 팝업을 부드럽게 표시하고 닫는 기능을 구현합니다.
 - Popup 폴더: 다양한 팝업 창을 관리하는 스크립트들로 구성되어 있습니다.
  - UIPopup_Onboarding.cs: 사용자가 처음 앱을 사용할 때 표시되는 온보딩 팝업을 관리합니다.
  - UIPopup_Error.cs: 오류 발생 시 표시되는 팝업을 관리합니다.
  - UIPopup_Settings.cs: 설정 화면을 팝업 형태로 제공하며 설정 내용을 관리합니다.
  - UIPopup_EpisodeList.cs: 에피소드 목록을 팝업 형태로 제공하고 이를 관리합니다.
  - UIPopup_Home.cs: 홈 화면의 팝업을 관리하며, 메인 인터페이스의 일부로 동작합니다.
 - Item 폴더: UI 아이템들을 관리하는 스크립트들로 구성되어 있습니다.
  - FAQItem.cs: 자주 묻는 질문(FAQ) 항목을 관리합니다.
  - CardEpisode.cs: 에피소드 카드를 UI에서 관리하고 업데이트하는 기능을 제공합니다.

3. Utils 폴더
 - PermissionCheck.cs: 앱 사용 중 필요한 권한을 확인하고 요청하는 기능을 제공합니다.
 - DontDestroyOnLoad.cs: 씬 전환 시에도 삭제되지 않도록 오브젝트를 유지하는 스크립트입니다.
 - Singleton.cs: 여러 클래스에서 싱글톤 패턴을 구현하여 전역적으로 관리할 수 있는 객체를 생성합니다.

4. Editor 폴더
 - PlayerPrefsClear.cs: Unity 에디터 내에서 PlayerPrefs를 초기화하는 도구를 제공합니다.
 - BuildTool.cs: 빌드 프로세스를 자동화하거나 간소화하는 도구를 제공합니다.
 - GameManager_Editor.cs: GameManager를 위한 커스텀 에디터 스크립트를 제공합니다.
