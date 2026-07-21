# ActionFit Lava Rush Installer

Lava Rush 콘텐츠 전체를 한 번에 설치하는 public Git UPM bootstrap 패키지입니다. 설치가 완료되면 bootstrap 패키지는 소비 프로젝트의 `Packages/manifest.json`에서 스스로 제거됩니다.

## 설치

Unity Package Manager의 `Add package from git URL`에서 아래 URL을 사용하거나 `Packages/manifest.json`에 등록합니다.

```json
{
  "dependencies": {
    "com.actionfit.lava-rush.installer": "https://github.com/ActionFit-Editor/LavaRushInstaller.git#0.1.13"
  }
}
```

Public repository에서 설치하므로 ActionFitGames private repository 접근 권한은 필요하지 않습니다.

## 강제 설치 묶음

- `com.actionfit.custompackagemanager@1.1.113`
- `com.actionfit.content-core@0.2.3`
- `com.actionfit.time@1.0.4`
- `com.actionfit.lava-rush@0.1.9`
- `com.actionfit.ui.foundation@2.0.4`
- `com.actionfit.lava-rush.ui@0.1.19`
- `com.coffee.ui-effect@5.10.8`
- `com.coffee.ui-particle@4.12.1`
- `com.coffee.softmask-for-ugui@3.5.0`
- `com.actionfit.uilighteffector@1.0.0` (`7dab46ec2378209bd1e524c8336b976eccb3df05` 고정)
- `jp.hadashikick.vcontainer@1.16.8` (UILighting 런타임 최소 버전; 이미 설치된 stable registry 동일/상위 버전은 보존)

UI와 원본 프리팹이 사용하는 네 시각 효과 패키지 및 UILighting의 VContainer 의존성은 선택 항목이 아닙니다. 활성 Lava Rush 번들은 위 열한 패키지를 모두 필수 상태로 소유하며, 누락이 감지되면 Custom Package Manager가 복구를 시도합니다. 단, embedded 패키지, `file:` dependency, fork, branch, 해석할 수 없는 revision, 사용자 변경값은 자동으로 덮어쓰지 않고 충돌로 보고합니다. 태그가 있는 저장소는 정확한 SemVer tag, 태그가 없는 UILighting 저장소는 전체 40자 immutable commit만 사용합니다.

프로젝트별 외형 작업이 필요하면 번들 설치가 끝난 뒤 Custom Package Manager에서 `com.actionfit.lava-rush.ui`만 **Embed for Edit**합니다. 0.1.19에는 원본 14개 역할 프리팹, 원본 PNG 56개, 필요한 시각 의존성과 TMP shader include, ReferenceBinding 계약과 engine 0.1.9 연결이 그대로 들어 있으며 Icon/Cell의 원본 `UI_Text` 설정, Cell `ScalePulse` Indicator, 중복 없는 로컬라이징 구성도 복구되어 있습니다. 정상 동작하는 베이스에서 이미지와 nested prefab을 프로젝트 스타일에 맞게 교체할 수 있습니다. AI 생성·placeholder·자동 대체 리소스는 베이스에 포함하지 않습니다. 이 전환은 자동으로 실행되지 않습니다. 호환되는 embedded UI는 설치·복구·제한 해제에서 보존되고, 요구 버전보다 오래된 embedded UI는 사용자 파일을 교체하지 않고 명시적 충돌로 남습니다. engine, Content Core와 Time은 downloaded 상태를 유지합니다. UI Foundation도 기본은 downloaded 상태지만, 서로 다른 global `UI_*` 구현을 이미 소유한 프로젝트는 로컬 스크립트를 삭제하지 않고 Foundation을 project-local로 Embed한 뒤 Runtime asmdef의 `autoReferenced`를 `false`로 격리해야 합니다.

## 설치와 복구

1. installer가 독립적으로 컴파일됩니다. `package.json`의 dependencies는 비워 둡니다.
2. Custom Package Manager가 없거나 오래된 API가 아직 로드돼 있으면 canonical `1.1.113` Git URL을 먼저 설치하고 새 API 등록까지 기다립니다.
3. 매니저가 `Editor/ContentBundleProfile.json`을 검사하고 원자적으로 manifest를 갱신합니다.
4. 모든 필수 패키지가 Unity Package Manager에 등록된 것이 확인되면 `ProjectSettings/ActionFitContentBundles.json`에 소유권을 기록합니다.
5. 마지막으로 `com.actionfit.lava-rush.installer` manifest 항목을 제거합니다.

중단된 설치·해제 transaction은 `UserSettings/ActionFitPackageManager/ContentBundleTransactions`에 남아 다음 Editor 시작 또는 패키지 등록 이벤트에서 복구됩니다. Unity batchmode에서는 inspect/plan만 허용하며 자동 쓰기를 수행하지 않습니다.

설치가 충돌로 중단되면 Console에는 패키지 ID와 credential-safe 현재/요구 dependency 요약이 함께 표시됩니다. `jp.hadashikick.vcontainer`는 profile이 명시적으로 허용하므로 stable registry `1.16.8` 이상을 보존하지만, floating Git URL이나 다른 package source는 자동으로 덮어쓰지 않습니다.

## 제한 해제

현재 GitHub CLI 로그인 `JewooSong`만 Custom Package Manager의 `Release Content Bundle`을 실행할 수 있습니다. 해제하면 installer가 독점 설치한 Lava Rush, UI, Content Core, Time 항목을 제거하거나 설치 전 값으로 되돌립니다. 다른 번들과 공유하는 항목, embedded 패키지, 사용자가 이후 변경한 항목, Custom Package Manager 자체는 보존합니다.

## 메뉴

- `Tools > Package > Lava Rush > Installer > Install or Repair Bundle`
- `Tools > Package > Lava Rush > Installer > README`

embedded 개발 패키지는 Editor 시작 시 자동 설치하지 않습니다. 위 메뉴를 명시적으로 실행한 경우에만 설치·복구를 시도합니다.

## 배포 경계

이 패키지를 프로젝트에 추가하는 것과 GitHub repository 생성, push, tag, catalog 등록, package publish는 별도 작업입니다.
