# RUMORS — The Throne Belongs to No One

> **"소문은 괴물을 낳고, 괴물은 왕좌를 노린다."**  
> Unity 6 Universal 2D 기반 일본풍 다크 판타지 픽셀 타이쿤 & 마왕 경영 시뮬레이션

---

## 📌 1. 프로젝트 개요

- **공식 타이틀**: RUMORS (구 명칭 RUMORBORNE 완전 폐기 및 네임스페이스 통일)
- **엔진 버전**: Unity 6 (6000.6.9f1) Universal 2D
- **플랫폼**: Windows PC (.exe)
- **아트 스타일**: 16×16 / 32×32 / 48×48 다크 판타지 픽셀 아트 (Loop Hero풍 고대 마왕성과 거친 도트 팔레트)
- **개발 언어**: C# (.NET / Unity C#)
- **입력 시스템**: `activeInputHandler: 2` (Both 모드 — 레거시 Input API로 안전하고 직관적인 조작 구현)

---

## 🏰 2. 핵심 시스템 및 아키텍처

### 2.1 네임스페이스 구조
모든 스크립트는 `Rumors.*` 네임스페이스로 정돈되어 있으며, 이전 명칭(Rumorborne) 잔여 코드가 0%입니다.
- `Rumors.Core`: 핵심 라이프사이클 및 시스템 매니저
  - `GameDefines.cs`: 열거형(BuildingType, MonsterSpecies, JobType, TileType 등) 및 공용 데이터 구조체 정의
  - `SpriteRegistry.cs`: `Assets/Resources/` 내 스프라이트/아이콘 자동 로드 및 런타임 캐싱
  - `GameManager.cs`: 하루 결산(`DailyReportData`), 경제 수지 정산, 전체 게임 루프 관리
  - `TimeManager.cs`: 1×, 2×, 4× 배속, 일시정지, 하루 게이지 진행, 날짜(년/월/일) 진행
  - `ResourceManager.cs`: 4대 자원(금화 Gold, 마력 Mana, 식량 Food, 광석 Ore) 실시간 관리 및 이벤트
  - `RumorManager.cs`: 소문 생성, 스택 축적, 실체화 가능 여부 판정
  - `DemonLordManager.cs`: 마왕 권위, 명성(Fame), 테크트리 해금 조건 연계
  - `HeroManager.cs`: 용사 출현 및 위협도 관리
  - `SaveLoadManager.cs`: JSON 기반 게임 세이브/로드 및 데이터 지속성
  - `GameBootstrapper.cs`: 씬 시작 시 모든 싱글톤 매니저, 카메라, 그리드, UI를 자동 초기화
- `Rumors.World`: 타일맵 및 월드 상호작용
  - `WorldGrid.cs`: 25×25 그리드, 중앙 마왕성 `(12, 12)`, 타일별 건설 가능성 판정
  - `TilemapVisualizer.cs`: 프로그래밍 타일 렌더러 및 마우스 호버 하이라이트
  - `CameraController.cs`: WASD/화살표 팬, 휠 줌(Pixel Perfect), 마우스 드래그 이동
  - `WorldClickController.cs`: 월드 클릭 시 몬스터(1순위) → 건물(2순위) → 빈 타일(건설창) 순서로 판정
- `Rumors.Buildings`: 시설 운영 및 건설
  - `BuildingData.cs`: 정적 건물 데이터 (비용, 유지비, 작업자 수, 생산량, 필요 명성 해금 조건)
  - `Building.cs`: 개별 건물 인스턴스 (레벨, 건설 애니메이션, 작업자 리스트, 생산 수지)
  - `BuildingManager.cs`: 타일 배치, 건설 시간 진행, 철거, 작업자 배속/해제
- `Rumors.Monsters`: 몬스터 AI 및 라이프사이클
  - `MonsterData.cs`: 개체 고유 ID, 스탯(HP, ATK, DEF, Speed), 성격, 세대, 충성도, 피로, 야망, 작업 지정
  - `MonsterAgent.cs`: 2D 자율 이동/배회(Wander), 지정 건물 출퇴근, 애니메이션(Idle/Walk/Attack) 제어
  - `MonsterManager.cs`: 몬스터 스폰, 목록 관리, 일일 피로/충성도 정산
- `Rumors.UI`: 다크 판타지 스타일 반응형 UI
  - `RumorUIHelper.cs`: 한글 폰트(`맑은 고딕`) 탑재, 반응형 캔버스/패널/버튼 헬퍼
  - `UIManager.cs`: 단일 캔버스 기반 8개 UI 컨트롤러 통합 초기화
  - `TopBarUI.cs`: 상단 자원(금/마/식/광), 소문 스택, 날짜, 배속 버튼, 세이브/로드
  - `BuildMenuUI.cs`: 타일 클릭 시 하단 슬라이드업 건설창 (해금/잠금 상태 및 조건 표시)
  - `EntityDetailUI.cs`: 건물 클릭 시 우측 슬라이드인 상세창 (일일 생산량, 작업자, 철거)
  - `MonsterPanelUI.cs`: 몬스터 클릭 시 상세 스탯, 성격, 특성 및 임무 변경 드롭다운
  - `MonsterListBarUI.cs`: 화면 하단 몬스터 아이콘 썸네일 바
  - `DailySettlementUI.cs`: 하루 종료 시 수입/지출 및 이벤트 로그 모달
  - `SceneNavigatorUI.cs`: 마왕성 클릭 시 하위 지역(저잣거리, 던전, 연구소) 이동 게이트
  - `RumorPanelUI.cs`: 우측 소문 목록 토글 및 실체화 확인창
- `Rumors.Editor`:
  - `SceneSetup.cs`: Unity 상단 메뉴 `RUMORS/Setup Scene`, `RUMORS/Run Tests`
  - `AssetGenerator.cs`: 픽셀 에셋 재구성 가이드

---

## 🎨 3. 제작 및 적용 완료된 상업용 픽셀 에셋 (`Assets/Resources/`)

`tools/generate_rumors_commercial_art.py` 엔진을 통해 생성된 100% 독창적인 클린룸(Cleanroom) 상업용 에셋이 적용되어 있습니다. 자세한 분석 및 권리 현황은 [ASSET_LICENSES.md](file:///c:/Users/lookm/Documents/GitHub/Rumors_Crown/Docs/ASSET_LICENSES.md)를 참조하십시오.

1. **지형 타일 (16×16 PPU 16)**:
   - `Tile_Ground_DarkGrass`, `Tile_Ground_Dirt`, `Tile_Ground_Stone`, `Tile_Ground_Swamp`
   - `Tile_Castle_Floor`, `Tile_Cobblestone` (자연스러운 박석길과 틈새 잔디 전이), `Tile_Highlight`
2. **건물 스프라이트 (32×32 / 48×48, 3단계 외형 변화 지원)**:
   - `Building_Castle`, `Building_Castle_Upgraded` (48×48 중앙 흑요석 마왕성)
   - `Building_Mine`, `Building_Mine_Upgraded` (지하 갱도 및 광차)
   - `Building_Farm`, `Building_Farm_Upgraded` (발광 포자 버섯 농장)
   - `Building_Dormitory`, `Building_Dormitory_Upgraded` (마족 숙소)
   - `Building_Warehouse`, `Building_Warehouse_Upgraded` (자원 창고)
   - `Building_ManaTower`, `Building_Training`, `Building_Construction` (공사 중 비계)
3. **몬스터 스프라이트 (32×32, 5종 9프레임 시퀀스)**:
   - `Monster_ImpScrap` (불꽃 꼬마 임프)
   - `Monster_MireSlime` (늪의 진흙 슬라임)
   - `Monster_BonePup` (뼈다귀 사냥개)
   - `Monster_GrimShadow` (어둠의 망령)
   - `Monster_GoblinTinker` (발명왕 고블린)
   - 각 몬스터별 단일 대표 스프라이트 및 `Idle (2f)`, `Walk (4f)`, `Attack/Work (3f)` 총 45프레임 시퀀스 완비
4. **UI 시스템 & 반응형 캔버스**:
   - `UI_Panel_9Slice` (16비트 고딕 석재 9-슬라이스 프레임)
   - `UI_Button_Normal`, `UI_Button_Hover`, `UI_Button_Pressed` (3상태 스프라이트 스왑)
   - 선명한 24×24 자원 아이콘 5종 (`Gold`, `Mana`, `Food`, `Worker`, `Rumor`)
   - `RumorUIHelper` 기반 드롭 섀도우 컴포넌트 탑재 및 맑은 고딕 가독성 확보
   - `SpriteRegistry.DefaultMaterial`(`Sprites/Default` Unlit) 적용으로 URP 조명 설정과 무관하게 100% 또렷한 원색 출력 보장

---

## 🚀 4. 시작 방법 (Quick Start)

1. Unity 6 (6000.6.9f1) 에디터에서 프로젝트를 엽니다.
2. Unity 상단 메뉴에서:
   - **`RUMORS` → `Setup Scene`** 클릭 (카메라 및 Bootstrapper 세팅)
   - **`RUMORS` → `Run Tests`** 클릭 (25×25 그리드 및 마왕성 위치 검증)
3. **Play (▶)** 버튼 클릭:
   - 월드 타일맵(25×25)과 중앙 마왕성이 자동 생성됩니다.
   - 초기 5마리의 몬스터가 마왕성 주변을 배회하며 작업을 수행합니다.
   - 빈 땅을 클릭하여 건설창을 열고 광산/농장/숙소를 배치할 수 있습니다.
   - 상단 바에서 1×/2×/4× 배속 제어 및 하루 진행, 저장/불러오기를 테스트할 수 있습니다.

---

## 📋 5. 개발 단계별 진행 현황 (Roadmap)

- [x] **Phase 0 — 시각적 기준 및 픽셀 규격 확정**
  - 고유 픽셀 아트 팔레트 적용, 타일 7종, 건물 8종, 몬스터 5종, UI 9Slice 구축
- [x] **Phase 1 — 플레이 가능한 도트 타이쿤 핵심 루프 완성**
  - 25×25 실제 타일맵 월드 및 마왕성 중심 배치
  - 직접 타일 클릭 건설, 건설 현장 타이머, 완성 연출
  - 몬스터 5종 자율 배회, 출퇴근 및 작업(채광, 농업, 훈련 등)
  - 4대 자원(금/마/식/광) 생산/소비 및 하루 결산 모달
  - 상단 HUD, 하단 몬스터 바, 세이브/로드(JSON)
- [ ] **Phase 2 — 마왕성 성장 & 소문 실체화 시스템 고도화**
  - 마왕성 레벨 2 업그레이드 연출 및 기능 해금
  - 소문 기록소 / 몬스터 추출기 기능 구현 (소문 스택 소모로 몬스터 소환)
  - 저잣거리 인간 NPC 시뮬레이션 및 소문 확산
- [ ] **Phase 3 — 몬스터 교배 & 혈통 유전자 시스템**
  - 교배 연구소, 부화장 구현
  - 부모 유전자 조합 및 돌연변이/유전병 로직
  - 절차적 외형 합성 (체형+부속기관+팔레트 스왑)
- [ ] **Phase 4 — 던전 관전 & 용사 자동전투 시스템**
  - 용사 파티 침입 스케줄러 및 정찰소
  - 2D 전장 자동전투 씬 전환 및 1x/2x/4x 관전 배속
- [ ] **Phase 5 — 파벌 정치 & 반란 시스템**
  - 충성도 0 몬스터의 독립 선언 및 왕좌 도전
- [ ] **Phase 6 — 왕좌 찬탈과 계승 (시대 연대기)**
  - 마왕 패배 시 왕관 낙하 연출 및 계승/잔당 선택
