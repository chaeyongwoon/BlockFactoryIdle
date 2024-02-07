# FrameWork

FrameWork

### 맨 처음 쓴다면! 

https://www.notion.so/mondayoff/FrameWork-035662d7b7f74f14b4dfe3404133c86d  사용법 있습니다!!!

- Scene 마다 해당 BaseScene을 상속받는 Scene오브젝트를 넣는다
- 씬에 있는 @Scene 오브젝트에 GameScene script 실행됨 > UI GameScene 호출 > GameManager.GameInit()   (!!!물론 원하는때에 넣으면 됩니다 이건 그냥 예시용!!!)
- 
- Managers 를 호출하면 Managers 저절로 생성됨
- Managers 에서 타겟프레임 60으로 잡았고 멀티 터치 안되게 막아놓았음!
- Managers 는 DontDestroy 이고 나머지 매니저는 Managers에서 생산함
- Game Manager만 예외로 MonBehaviour를 사용하여 오브젝트로 생성합니다. (!!디버깅의 편리함!!)
- Managers 안에 Update에서 인터넷 연결을 확인함 삭제 해도됩니다!
- 새로운씬으로 갈때마다 Managers에서 Clear가 호출됨
- Managers 누르면 DataManager Class만 인스펙터에서 관측이 가능함
-
- DataManger는 Managers.Data 로 호출되며 EasySave3을 사용함
- 
- 풀링하는 객체는 Poolable 스크립트를 달아주어야 함
- 풀링매니저는 웬만하면 안써도 됨, 리소스 매니저의 Instance와 Destroy를 활용할 것
- 
- 모든 리소스는 리소스 매니저를 활용해서 처리할 것!
- 인게임에서 리소스를 활용하고 싶다면 리소스 폴더 안에 넣고 활용
- 
- 씬 이동은 Scene 매니저를 활용함
- 씬 이동시에 SceneTrasition 애니메이션 실행 됨 (!!애니메이션은 바꿔서 활용할 것!! 있는것은 그냥 예시용!!)
- Define.Scene 에 씬들의 이름을 넣어주어야함
- 
- 사운드 관련내용은 Managers.Sound
- UI 관련 내용은 Managers.UI
- 버튼은 AddButtonEvent 함수를 사용하면 작았다 커지는 애니메이션을 사용함 ( Extension 에 AddButtonEvent 함수에 있습니다 )
- 
- UI는 Particle과 3D 오브젝트를 UI 에 넣기 위해서 RenderMode를 Camera로 사용함
- 카메라는 하이어라키에 있으면 자동으로 들어가고 없으면 런타임에서 만들어서 사용합니다.
- UI에 넣는 파티클의 Layer를 UI로 사용합니다.
- 
- 해보고 모르겠으면 문의!!
- 
- Utils 폴더에는 여러 스크립트가 있는데
- Define 은 상수정의나 Enum정의때 사용
- Extension은 Extension기능을 위한 기능 추가 모음집
- Util 은 아무곳에나 활용가능한 Static 함수 집합
- 
- Build 탭에서 적절한 것으로 빌드 하기
- Build 하기전에 항상 Project Setting > Player > Other Settings에 Version / Bundle Version Code / Build 확인!!
- 안드로이드는 데스크탑 /Builds/Android/ 폴더로 만들어짐 ( Jenkins 때문에 내 폴더로 바꾼다면 AutoBuilder 스크립트에서 TARGET_DIR 바꾸어주기 )
- iOS는 데스크탑 /Builds/iOS/ 폴더로 만들어짐 ( Jenkins 때문에 내 폴더로 바꾼다면 AutoBuilder 스크립트에서 APP_FOLDER 바꾸어주기 )
- 
- Jerry 탭에 MakeCursor 누르면 커서 f 눌러서 손으로 커서 교체가능
- Jerry 탭에 JoyStick 누르면 조이스틱 활용 가능
- 조이스틱 움직이는것은 아래에 길이 있는지 Walkable Layer로 되어있는 무언가가 있는지 (collider 필요 , Running Game 방식은 상관없음)
- JoystickController 에 CanMove 가 True 상태여야 움직임 ( Manager.Game.JoyStickController.CanMove 로 접근가능 )
- Jerry 탭에 Create UI Attractor 는 Hierachy 에서 오브젝트 하나 눌러서 그 아래에 UI Attractor 생성 ( Canvas 안에다가 넣을것 ) 
- 
- UI Particle 은 Canvas가 Screen Space - Overlay 인 경우 Particle을 활용하기 위해 파티클 상위 하이어라키에 붙여서 활용
- 
- Anti-Cheat Toolkit 은 메모리 탐지에 조금 더 안전하도록 하는 에셋  https://mentum.tistory.com/390- 
- 
- DOTween > Using DG.Tweeing 으로 사용
-  
- EasySave에 대해 정리 되있는 링크 https://mentum.tistory.com/156 
- 
- Editor Console Pro > 그냥 Unity Console보다 기능이 많음
- 
- Maintainer는 사용하는 레퍼런스를 알려주고 정리하는 에셋
- 
- 프로젝트 탭에서 폴더 Alt + 클릭 하면 폴더 색 바꿀수 있음 (Rainbow Folder2)
- 
- 씬에서 우클릭시 겹쳐있는 오브젝트 리스트가 나옴 (Selection Utility)
- 
- True Shadow 는 Shadow나 outline 대신 사용 가능


### Imported Assets

- Anti-Cheat Toolkit Version 2021.6.3 - December 19, 2022
- DOTween          Version 1.0.335 - October 10, 2022
- Easy Save        Version 3.5.3 - December 12, 2022
- Editor Console Pro Version 3.971 - April 06, 2022
- Maintainer       Version 1.16.0 - September 19, 2022
- Rainbow Folder2  Version 2.4.0 - May 07, 2022
- Selection Utility Version 1.2 - April 20, 2021
- True Shadow      Version 1.4.4 - December 14, 2022




### 업데이트 리스트 

v 1.0.1
- 조이스틱 제리탭으로 올라감
- 몇몇 버그 Fix

v 1.0.2
- UI particle 추가 ( 참조 : https://github.com/mob-sakai/ParticleEffectForUGUI )
- UI particle 추가 시에 Mobile 쉐이더로 바꾸기 위해서 Set Material 버튼이 생김 (overlay에서 그냥 쉐이더가 안됨...)
    UI/Unlit/Transparent 쉐이더 머테리얼을 만들어 적용시킴
- UI Attractor 추가 (Jerry 탭  동전 먹거나 하는 연출때 사용하셈)


v 1.0.5
- CursorManager 손가락 추가(1~9)
- JoystickController : Update -> FixedUpdated 변경.
- JoystickController : CheckButtonClick() 제거 - UI 클릭 확인 기능
- 단축키 [ - ]키로 EasySave 데이터 초기화 가능.
- Woody Color 추가.

v 1.1.0
- [DOTween Pro] [Anticheat] [EasySave] is updated the latest version.
- Added Property 'Speed' in JoyStickEditor.cs 

    case JoyStickMethod.RunningGame:
    _joystickController.Speed = EditorGUILayout.FloatField(new GUIContent("Speed", "속도"), _joystickController.Speed); 

-