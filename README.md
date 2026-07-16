# ActionFit Lava Rush Installer

Lava Rush 콘텐츠 전체를 한 번에 설치하는 public Git UPM bootstrap 패키지입니다. 설치가 완료되면 bootstrap 패키지는 소비 프로젝트의 `Packages/manifest.json`에서 스스로 제거됩니다.

## 설치

Unity Package Manager의 `Add package from git URL`에서 아래 URL을 사용하거나 `Packages/manifest.json`에 등록합니다.

```json
{
  "dependencies": {
    "com.actionfit.lava-rush.installer": "https://github.com/ActionFit-Editor/LavaRushInstaller.git#0.1.2"
  }
}
```

Public repository에서 설치하므로 ActionFitGames private repository 접근 권한은 필요하지 않습니다.

## 강제 설치 묶음

- `com.actionfit.custompackagemanager@1.1.96`
- `com.actionfit.content-core@0.2.1`
- `com.actionfit.time@1.0.3`
- `com.actionfit.lava-rush@0.1.3`
- `com.actionfit.lava-rush.ui@0.1.3`

UI는 선택 항목이 아닙니다. 활성 Lava Rush 번들은 위 다섯 패키지를 모두 필수 상태로 소유하며, 누락이 감지되면 Custom Package Manager가 복구를 시도합니다. 단, embedded 패키지, `file:` dependency, fork, branch, 해석할 수 없는 revision, 사용자 변경값은 자동으로 덮어쓰지 않고 충돌로 보고합니다. 동일 canonical repository의 낮은 tag만 요구 버전으로 올리고, 같거나 높은 tag는 보존합니다.

## 설치와 복구

1. installer가 독립적으로 컴파일됩니다. `package.json`의 dependencies는 비워 둡니다.
2. Custom Package Manager가 없으면 canonical `1.1.96` Git URL을 먼저 설치합니다.
3. 매니저가 `Editor/ContentBundleProfile.json`을 검사하고 원자적으로 manifest를 갱신합니다.
4. 모든 필수 패키지가 Unity Package Manager에 등록된 것이 확인되면 `ProjectSettings/ActionFitContentBundles.json`에 소유권을 기록합니다.
5. 마지막으로 `com.actionfit.lava-rush.installer` manifest 항목을 제거합니다.

중단된 설치·해제 transaction은 `UserSettings/ActionFitPackageManager/ContentBundleTransactions`에 남아 다음 Editor 시작 또는 패키지 등록 이벤트에서 복구됩니다. Unity batchmode에서는 inspect/plan만 허용하며 자동 쓰기를 수행하지 않습니다.

## 제한 해제

현재 GitHub CLI 로그인 `JewooSong`만 Custom Package Manager의 `Release Content Bundle`을 실행할 수 있습니다. 해제하면 installer가 독점 설치한 Lava Rush, UI, Content Core, Time 항목을 제거하거나 설치 전 값으로 되돌립니다. 다른 번들과 공유하는 항목, embedded 패키지, 사용자가 이후 변경한 항목, Custom Package Manager 자체는 보존합니다.

## 메뉴

- `Tools > Package > Lava Rush > Installer > Install or Repair Bundle`
- `Tools > Package > Lava Rush > Installer > README`

embedded 개발 패키지는 Editor 시작 시 자동 설치하지 않습니다. 위 메뉴를 명시적으로 실행한 경우에만 설치·복구를 시도합니다.

## 배포 경계

이 패키지를 프로젝트에 추가하는 것과 GitHub repository 생성, push, tag, catalog 등록, package publish는 별도 작업입니다.
