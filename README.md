# 이세계 용사

![image](https://github.com/user-attachments/assets/837b6fc7-1a05-42c8-957b-01f547661572)



# 📄프로젝트 정보
#### 장르
3D RPG - 이세계로 떨어진 학생이 용사가되어 마을을 구하는 스토리.

#### 참여인원
개발자 1인

#### 실행 영상
https://youtu.be/XW64HEdqGC8



# 📝사용기술
1) 클라이언트 서버 간 통신
   - HTTP통신을 이용한 사용자 인증 및 게임 데이터 전달 구현.
   - RESTful API를 통해 로그인, 회원가입, 퀘스트 저장 및 불러오기 기능 구현.
   - Unity Task와 async를 활용해 비동기 처리 방식으로 제작. 
2) DB 설계 및 PostSQL를 이용한 DB 구현
3) 디자인 패턴을 활용한 관리
   - 싱글톤 패턴을 활용한 공통기능 관리
     - 퀘스트, 스토리 등 공통 기능을 싱글톤 패턴을 활용해 구현. [퀘스트 관리 스크립트](https://github.com/pjsjyh/anotherRPG/blob/main/3dMMO/Assets/scripts/Quest/QuestManager.cs)
   - 상태 패턴을 활용해 게임 관리
     - enum과 switch를 활용한 게임 관리 [몬스터 스크립트](https://github.com/pjsjyh/anotherRPG/blob/main/3dMMO/Assets/scripts/GameComponent/Monster/Monster.cs)
4) 효율적인 코드 구성
   - 기능별 책임 분리를 통해 유지보수성 확보 [기능을 메서드로 구분하여 확장이 용이하도록 설계](https://github.com/pjsjyh/anotherRPG/blob/main/3dMMO/Assets/scripts/Quest/QuestManager.cs)
   - UniRx를 활용해 상태 변화에 따른 UI 자동 갱신 [데이터 셋팅](https://github.com/pjsjyh/anotherRPG/blob/main/3dMMO/Assets/scripts/GameComponent/Player/CharacterInfo.cs)
5) 데이터 관리
   - 데이터 소실을 고려한 로컬 저장 [로컬 저장 스크립트](https://github.com/pjsjyh/anotherRPG/blob/main/3dMMO/Assets/scripts/GameComponent/Player/CharacterRepository.cs)
  
# 구현 기능
#### 1) 회원가입 및 로그인
#### ![image](https://github.com/user-attachments/assets/777df3a7-fd9c-4719-8bc0-5c5aeb5a3430)
   - 로그인과 회원가입 기능 제작.
   - 같은 아이디, 이메일 외에 회원가입 가능하도록 구현. 패스워드는 저장 할 때 암호화처리하여 저장되도록 구현.
#### 2) 로딩 화면
#### ![image](https://github.com/user-attachments/assets/d22b5f8b-1dc5-4fb1-9938-f5f67e01f8ab)
   - 게임이 시작되기전 셋팅을 위한 화면
   - 이 과정에서 클라이언트-서버가 통신하여 캐릭터 데이터 셋팅을 진행한다. 또한 퀘스트나 기본적인 셋팅 모두 진행.
#### 3) 퀘스트 셋팅
#### ![image](https://github.com/user-attachments/assets/699fb778-4a3c-4788-a1c6-51955a98b5ca)
   - 시작시 새로운 계정은 자동으로 스토리가 진행되도록 제작.
   - 로딩될 때 퀘스트 목록과 현재 받았던 퀘스트 목록을 합쳐 셋팅진행.
#### ![image](https://github.com/user-attachments/assets/d5fabee9-2908-44ee-ae01-94bab56820c4)
   - 퀘스트를 받을 땐 항상 다음과 같은 창으로 퀘스트 받기 창이 뜬다.
#### ![image](https://github.com/user-attachments/assets/94e2dbab-bfb6-43a5-9487-2acd47d39cf7)
   - 퀘스트를 받을 수 있는 NPC가 있다면 UI 표출.
   - 좌측에 받은 퀘스트 목록 띄워주기.
   - 받은 퀘스트가 있다면 로그인 시 불러와 셋팅.
#### 4) 플레이어
#### - 플레이어 스킬
#### ![image](https://github.com/user-attachments/assets/8141b77d-050a-4827-adf2-e13001f10fee)
   - 글로벌 스킬, 논글로벌 스킬 나누어 구현. (글로벌: 쿨타임이 같이 체크되는 스킬, 논글로벌: 별도로 쿨타임을 가지는 스킬)
   - 공격스킬을 쓸 때만 공격판정이 될 수 있도록 평소에는 피격확인 콜라이더 off.
#### - 플레이어 움직임
   - 키보드 클릭을 통해 이동할 수 있도록 제작.
#### 5) 카메라
   - 카메라가 플레이어를 따라다닐 수 있도록 구현.
   - 장애물이 있는지 없는지에 따라 위치 변화.
#### 6) 몬스터
#### ![image](https://github.com/user-attachments/assets/b6507a65-c8b0-4489-909c-a9efac9beb36)
   - 범위 내에 플레이어가 들어오면 추적.거리가 멀어지면 추적 종료.
   - 장판 형식의 공격 타입.
   - 피격시 빨간 효과 표출.
