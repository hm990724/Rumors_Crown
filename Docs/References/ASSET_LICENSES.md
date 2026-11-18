# RUMORS 에셋 라이선스 및 상업적 권리 관리 대장 (Asset Licenses & Cleanroom IP Registry)

본 문서는 Steam 유료 출시를 목표로 개발 중인 일본풍 다크 판타지 마왕 경영 시뮬레이션 **RUMORS**의 모든 시각 그래픽, 타일셋, 스프라이트, UI, 사운드, 폰트 에셋의 저작권 및 상업적 라이선스 현황을 관리하는 공식 문서입니다.

---

## 1. 지식재산권(IP) 보호 및 클린룸(Cleanroom) 제작 원칙

1. **상업 출시용 100% 오리지널 에셋**:
   - 본 프로젝트의 모든 비주얼 에셋은 상업용 유료 배포에 법적 하자가 없도록 철저한 클린룸 프로세스를 준수하여 제작되었습니다.
   - 외부 레퍼런스 이미지의 픽셀을 자르거나(Crop), 트레이싱(Tracing)하거나, 단순 색상 변형/좌우 반전하여 사용하는 행위를 엄격히 금지합니다.
2. **벤치마킹 분석과 독창적 재설계의 분리**:
   - `Docs/References/`의 5종 스크린샷에서 실제 색상 팔레트(따뜻한 올리브 잔디 `#999F18`, 부드러운 박석 `#B2AF86`, 목재 프랍 `#A58156`, 발광 푸른 룬 `#6FADB9`) 및 고유 프랍 구성을 심층 추출하였습니다.
   - 단순 사각형 형태의 프로토타입을 완전히 탈피하고, `tools/generate_rumors_commercial_art.py` 엔진을 통해 건물마다 고유한 실루엣(광산 갱구, 버섯 온실, 화물 창고, 마력탑, 훈련 과녁)과 2.5등신 다크 판타지 몬스터 5종을 독창적으로 제작했습니다.

---

## 2. 레퍼런스 이미지 5종 벤치마킹 분석 보고 (`Docs/References/`)

| 레퍼런스 식별자 | 시각적 특징 및 벤치마킹 분석 | RUMORS에 독창적으로 적용된 디자인 요소 |
| :--- | :--- | :--- |
| **Reference 1**<br>(전체 마을 조망) | • 탑다운 3/4 시점 (Top-Down 3/4 Oblique)<br>• 좌상단(Top-Left) 일관된 광원 모델<br>• 따뜻한 올리브-머드-슬레이트 톤 팔레트<br>• 부드러운 박석길(Flagstones)과 틈새 잔디 전이 | • 흑요석 고딕 마왕성(48×48)의 웅장한 아치형 철제 쇠살문과 수정 첨탑<br>• 순수 검은색 외곽선 대신 재질별 딥 톤 음영선 채택<br>• 둥글둥글한 박석(Flagstone) 타일 틈새로 자연스럽게 돋아난 잔디 및 야생화 도트 배치 |
| **Reference 2**<br>(주거 구역 및 농장) | • 흙길과 잔디의 부드러운 유기적 경계<br>• 건물 증축에 따른 외형 변화<br>• 농지 밭이랑의 명암 대비와 목조 울타리 | • 버섯 농장(Farm): 청록·보라·황금빛 발광 버섯 밭과 스트라이프 천막 어닝, 물양동이<br>• 숙소(Dormitory): 2층 목조 테라스, 따뜻한 창문 불빛, 1층 나무 벤치, 빨랫줄, 굴뚝 연기<br>• 미완공(공사 중) 비계(Scaffolding)와 붉은 깃발 분리 |
| **Reference 3**<br>(작업 시설 및 광산) | • 광차(Minecart), 레일, 지반 굴착 단차 표현<br>• 석재 작업대와 공구 프랍 밀도 | • 광산(Mine): 깊은 바위 언덕 속 갱도 터널 입구, 두꺼운 목조 보, 철제 레일, 광차에 담긴 보랏빛/황금빛 원석 더미, 모루와 망치, 가로등 램프<br>• 업그레이드 시 상단 도르래 크레인과 수정 덩어리 증축 반영 |
| **Reference 4**<br>(실내 및 디테일 프랍) | • 나무 상자(Crate), 오크통(Barrel), 횃불 불빛<br>• 16비트 질감의 가구와 바닥 석재 디테일 | • 창고(Warehouse): X자 보강 나무상자 3~4개 적재, 오크통 2개, 덮개 방수포, 손수레<br>• 마력탑(ManaTower): 룬 문자가 은은히 빛나는 흑요석 기둥과 상단 부유하는 거대 보랏빛 마력 다이아몬드 |
| **Reference 5**<br>(캐릭터 및 활동) | • 2.5등신 귀여운 실루엣의 캐릭터<br>• 이동 시 톡톡 튀는 바운스 애니메이션, 훈련장 프랍 | • 훈련장(Training): 사격용 과녁판, 짚을 묶은 훈련용 허수아비 타겟 인형, 무기 걸이 랙<br>• 5종 몬스터: 임프(날개+뿔+횃불), 슬라임(에메랄드 푸딩+영혼핵), 본펍(해골늑대+도깨비불 눈), 그림섀도우(후드망토+외눈), 고블린(고글+망치+등짐) 고유 실루엣 완비 |

---

## 3. 에셋 현황 및 권리 대장 (Commercial Asset Manifest)

| 카테고리 | 리소스 파일 경로 | 사양 및 프레임 | 저작권 / 라이선스 | 상업 출시 적합성 | 비고 |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **지형 타일** | `Assets/Resources/Tiles/` | 16×16 PPU 16, Point Filter | **100% Original Script-Generated** (Engine: `generate_rumors_commercial_art.py`) | **상업용 즉시 사용 가능 (Verified Cleanroom)** | DarkGrass, Cobblestone, CastleFloor, Dirt, Stone, Swamp, Highlight 7종 |
| **건물 스프라이트** | `Assets/Resources/Buildings/` | 32×32 ~ 48×48 PPU 16 | **100% Original Script-Generated** | **상업용 즉시 사용 가능 (Verified Cleanroom)** | Castle, Mine, Farm, Dormitory, Warehouse (기본 + Upgraded 2단계 외형), ManaTower, Training, Construction 총 13종 완전 차별화 |
| **몬스터 스프라이트** | `Assets/Resources/Monsters/` | 32×32 PPU 16 (대표 + 프레임) | **100% Original Script-Generated** | **상업용 즉시 사용 가능 (Verified Cleanroom)** | ImpScrap, MireSlime, BonePup, GrimShadow, GoblinTinker 5종 (Idle 2f, Walk 4f, Attack 3f 총 45프레임) |
| **UI 에셋** | `Assets/Resources/UI/` | 9-Slice Sliced Sprite & Icons | **100% Original Script-Generated** | **상업용 즉시 사용 가능 (Verified Cleanroom)** | UI_Panel_9Slice (32×32 고딕 석재 프레임), UI_Button 3상태(Normal/Hover/Pressed), 자원 아이콘 5종 |
| **폰트 (Font)** | `Assets/Resources/Fonts/` (Unity 내장 / System Fallback) | 한국어 지원 고딕 | OS System Fallback / OFL 권장 | `RumorUIHelper`에서 `맑은 고딕` 드롭 섀도우 컴포넌트로 선명하게 렌더링 |

---

## 4. 저작권 검증 체크리스트

- [x] 타 게임 또는 상용 에셋 스토어의 스프라이트를 무단 추출/도용하지 않았는가? → **확인 완료 (0% 도용, 순수 오리지널 생성)**
- [x] 레퍼런스 이미지(5종)의 픽셀 트레이싱이나 부분 복제가 일체 없는가? → **확인 완료 (클린룸 이론 분석 후 독자 제작)**
- [x] 모든 건물의 외형과 실루엣이 확연히 구분되는가? → **확인 완료 (광산 갱구, 버섯 온실, 화물 창고, 훈련 과녁 등 고유 디자인 완비)**
- [x] 몬스터들의 형태가 단순 사각형이 아닌 생명체다운 디테일을 갖추었는가? → **확인 완료 (날개, 고글, 뼈 갈비뼈, 영혼핵, 횃불 등 반영)**
- [x] Steam 유료 출시 시 저작권 침해 분쟁 소지가 없는가? → **확인 완료 (자체 제작 스크립트 소스코드 보존)**
