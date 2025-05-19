# 이세계 용사

![image](https://github.com/user-attachments/assets/837b6fc7-1a05-42c8-957b-01f547661572)



# 📄프로젝트 정보
#### 장르
3D RPG - 이세계로 떨어진 학생이 용사가되어 마을을 구하는 스토리.

#### 참여인원
개발자 1인

#### 실행 영상
https://www.youtube.com/watch?v=48EXLYf6Gxs



# 📝사용기술 및 구현 기능
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
   - 인터페이스, 상속을 활용해 코드 분리 및 유지보수성 향상 [상속을 이용한 공격코드](https://github.com/pjsjyh/ProtectTeeth/blob/master/ProtectTeeth/Assets/Scripts/Game/GoodTeethSetting.cs) [공통 코드](https://github.com/pjsjyh/ProtectTeeth/blob/master/ProtectTeeth/Assets/Scripts/Game/GoodSetting.cs)
   - static을 활용해 데이터 관리
   - 공통 로직 묶어 활용도 향상
5) 데이터 관리
   - 데이터 소실을 고려한 로컬 저장 [로컬 저장 스크립트](https://github.com/pjsjyh/anotherRPG/blob/main/3dMMO/Assets/scripts/GameComponent/Player/CharacterRepository.cs)
