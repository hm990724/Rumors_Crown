# RUMORS Expansion Pack — Unity 6용

## 무엇을 받았나
- `Assets/_RUMORS_Expansion` 한 폴더만 기존 Unity 프로젝트의 `Assets` 아래로 복사
- `RUMORS/Expansion/Create Isolated Demo Scene` 메뉴 클릭
- 생성된 `Assets/_RUMORS_Expansion/RumorsExpansionDemo.unity` 열고 Play
- Space 일시정지, 1/2/3 키가 각각 1×/2×/4×
- 오른쪽 테스트 버튼으로 교배, 실체화, 저장, 불러오기 시험

**주의:** 기존 `Assets/Scenes/DemonCastle.unity`나 `TimeManager.cs`를 덮어쓰지 않는다. 현재 프로젝트와 통합하려면 원본 Unity 코드 검사가 선행되어야 한다.

## 에셋 안내
- `Art/Buildings/RUMORS_Castle_Concept_RGBA.png`: AI 생성 성채 원화에서 체크 배경을 반자동 제거한 시안. 작은 픽셀 게임 스프라이트 최종 검수 필요
- `Art/Buildings/RUMORS_Castle_Preview_192.png`: 단순 축소 미리보기. 완성된 수작업 도트라고 주장하지 않음
- `Art/Concept/WorldArtTarget_*.png`: **레이아웃·분위기 레퍼런스**로만 사용; 실제 Unity UI는 아님
- `Art/UI/RUMORS_Clock_Ring.png`: 직접 제작한 원형 시계 이미지
- `Art/UI/RUMORS_Panel_Frame_64.png`: 직접 제작한 픽셀 UI 프레임

## UI 시계 적용
`RUMORS_Clock_Ring.png`을 Unity의 `Assets/Resources/RUMORS_Clock_Ring.png`에 복사하고 `Sprite (2D and UI)`, Filter Mode `Point`로 임포트하면 독립 데모 UI가 불러온다. 복사하지 않으면 기본 색상의 원형 대체 UI가 나온다.

## Input 문제 해결
패키지의 데모는 구 Input API를 사용한다. Unity `Project Settings > Player > Active Input Handling`이 **Both** 또는 **Input Manager (Old)**일 때만 입력이 동작한다. 기존 프로젝트의 설정과 일치시키거나 새 Input System 리팩터링이 필요하다.

## 한계
코드의 구조는 작성했으나 Unity 에디터 실행은 이 환경에서 검증하지 못했다. 원본 GitHub 저장소를 통째로 내려받는 데 실패해 기존 코드와의 자동 병합은 보장하지 않는다. 적용 전 Git 커밋을 남겨라.
