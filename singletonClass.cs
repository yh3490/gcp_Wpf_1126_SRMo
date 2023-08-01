using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace gcp_Wpf
{
    public struct Gcp_Info
    {
        // define fields and properties for the shared struct here
        public int srmCount { get; set; }

        public int language { get; set; }
    }


    public struct Srm_Info
    {
        // define fields and properties for the shared struct here

        // Srm Info
        public int srmID { get; set; }
        public int srmType { get; set; }
        public int forkType { get; set; }

        // Rack Info
        public int row { get; set; }
        public int bay { get; set; }
        public int lev { get; set; }
        public int stn { get; set; }

        // Comm Info
        public string srmIP { get; set; }
        public int srmPORT { get; set; }

        // Rtu Info
        public int hostPORT { get; set; }
        public string comPORT { get; set; }
        public int baudRate { get; set; }
        public int parity { get; set; }
        public int dataBit { get; set; }
        public int stopBit { get; set; }

        // Select State         - UI 내 사용선택 정보 구동 시 에만 저장 - 저장안함
        public bool bUse_fork1 { get; set; }
        public bool bUse_fork2 { get; set; }
    }

    public struct Gcp_State         // 지상반 호기별 상태정보 저장
    {
        public byte gcpMode;        // 지상반 모드 Return 값
        public bool safetyPlug;     // 
        public bool faultReset;     // DI 리셋버튼 입력  or 프로그램 리셋버튼 클릭
        public bool emStop;         // B접점으로 DI접점은 1이 정상 ---- 통신 송신은  안눌림(정상):0  눌림(비상):1 으로 처리
    }
    
    unsafe public struct Srm_State         // 크레인 호기별 상태정보 저장
    {
        public byte protocolVer;        // 프로토콜 버전
        public byte firmwareVer;        // 펌웨어 버전  10 = Ver1.0
        public byte buildYear;          // 펌웨어 빌드연도
        public byte buildMonth;         // 펌웨어 빌드 월
        public byte buildDay;           // 펌웨어 빌드 일
        public uint utcTime;            // UTC Time
        public fixed byte projNo[6];    // 프로젝트 번호
        public byte groupNo;            // 그룹 번호
        public ushort srmNo;            // 호기 번호
        public ushort srmType;          // 장비타입

        // 지상반송신 상태 수신 정보확인용
        public Gcp_State gcpRecvState;

        public fixed byte CVOK[8];      // 통신 인터록  (CV->장비)  스테이션 당 1bit사용  8*8 64개 스테이션 사용
        public fixed byte CVNO[8];      // 통신 인터록  (장비->CV)  

        public byte devMode;            // Bit7(128): 자동모드, Bit6(64): 수동모드, Bit5(32): 강제모드, Bit4(16): 셋업모드 ----- OFF:0, ON:1

        public byte devState1;          // Bit7(128): 시작상태, Bit6(64): 일시정지, Bit5(32): 비상정지, Bit3(8): 인버터 접속상태, Bit2(4): 장치 이상상태, Bit1(2): 장치 경고상태
        public byte devState2;          // Bit7(128): 비상정지 스위치, Bit6(64): 자동/수동 스위치 - 자동:0 수동:1
        public byte operCode;           // 장비 동작코드
        public byte errcodeH;           // 이상코드 대분류
        public byte errcodeM;           // 이상코드 중분류
        public ushort errcodeL;           // 이상코드 소분류

        public FORK_State fork1;
        public FORK_State fork2;

        public TRAV_State trav;
        public LIFT_State lift;

        // Task 작업정보
        public uint taskNo;
        public byte taskState;

        public fixed byte dInput[16];
        public fixed byte dOutput[5];

        public byte invContInput;
        public byte invContOutput;

        public byte invLiftInput;
        public byte invLiftOutput;

        public byte invTrav1Input;
        public byte invTrav1Output;

        public byte invTrav2Input;
        public byte invTrav2Output;

        public byte invForkInput;
        public byte invForkOutput;
    }
    
    public struct FORK_State
    {
        public byte curStation;           // 포크 스테이션
        public ushort curBay;
        public byte curLev;
        public sbyte curPosNum;             //  F정 = (-3) = M정 = (-2) = H정 = (-1) = C정 = (1) = H정 = (2) = M정 = (3) = F정      - 포크 중간 중간 포지션 위치 값 표시  Signed
        public byte curPos1;                //  Bit7-2
        public byte curPos2;                //  Bit7-1   7:C정 6:좌정1 5:좌정2 4:좌정3 3:우정1 2:우정2 1:우정3

        public byte targetStation;
        public byte targetRow;
        public byte targetLev;
        public ushort targetBay;

        public byte state1;                 // Bit5: 원점확인, Bit4: 정위치, Bit3: 이동방향 (좌:0, 우:1), Bit2: 감속상태 (정지/등속:0, 감속:1), Bit1: 가속상태 (감속동일), Bit0: 동작상태
        public byte state2;                 // Bit3: 인버터접속상태, Bit2: 부하 튜닝 중, Bit1: 무부하 튜닝 중, Bit0: 홈복귀

        public byte loadType;               // 적재 화물타입
        public int curPos;
        public short curSpd;
        public int targetPos;

        // 작업정보 - 반송명령 
        public uint jobNo;
        public byte taskIdx;
        // From     5Byte
        public byte fromStation;
        public byte fromRow;
        public ushort fromBay;
        public byte fromLev;
        // To       5Byte
        public byte toStation;
        public byte toRow;
        public ushort toBay;
        public byte toLev;

        public byte cmdCode;
        public byte procState;
        public byte procStep;

        // 작업정보 - 이동작업
        public uint mvJobNo;
        public byte mvToStation;
        public byte mvToRow;
        public ushort mvToBay;
        public byte mvToLev;

        public byte mvProcState;
        public byte mvProcStep;
    }

    public struct TRAV_State
    {
        public byte state1;                 // Bit5: 원점확인, Bit4: 정위치, Bit3: 이동방향 (전진:0, 후진:1), Bit2: 감속상태 (정지/등속:0, 감속:1), Bit1: 가속상태 (감속동일), Bit0: 동작상태
        public byte state2;                 // Bit3: 인버터접속상태, Bit2: 부하 튜닝 중, Bit1: 무부하 튜닝 중, Bit0: 홈복귀
        public byte fwDecNo;
        public byte bwDecNo;

        public int curPos;
        public short curSpd;
        public int targetPos;
    }

    public struct LIFT_State
    {
        public byte state1;                 // Bit5: 원점확인, Bit4: 정위치, Bit3: 이동방향 (상승:0, 하강:1), Bit2: 감속상태 (정지/등속:0, 감속:1), Bit1: 가속상태 (감속동일), Bit0: 동작상태
        public byte state2;                 // Bit3: 인버터접속상태, Bit2: 부하 튜닝 중, Bit1: 무부하 튜닝 중, Bit0: 홈복귀
        public byte upDecNo;
        public byte dnDecNo;

        public int curPos;
        public short curSpd;
        public int targetPos;
    }

    public struct Str_RecvHeader
    {
        public byte srcType;
        public byte srcID;
        public byte dstType;
        public byte dstID;
        public byte seqNum;
        public byte byPass1;
        public byte byPass2;
        public byte cmd1;
        public ushort len; //DATA 길이 + 1 (CMD2 길이)
        public byte cmd2;
        public byte[] data;
    }

    public struct Str_SendHeader
    {
        public byte srcType;
        public byte srcID;
        public byte dstType;
        public byte dstID;
        public byte seqNum;
        public byte byPass1;
        public byte byPass2;
        public byte cmd1;
        public ushort len; //DATA 길이 + 1 (CMD2 길이)
        public byte cmd2;
        public byte[] data;
    }
    public struct Srm_Packet
    {
        public bool manClicked;         //  수동버튼 클릭여부 확인
        public int manCmd;
        public int manAxis;
        public byte manTrav;
        public byte manLift;
        public byte manFork1;
        public byte manFork2;

        // Received Packet
        public Str_RecvHeader recvStr;
        public Str_SendHeader sendStr;
    }

    public struct Dio_Packet
    {
        public ushort[] DI;    // to do - Unsafe 고정버퍼로 할지...
        public ushort[] DO;
    }

    public struct SharedStruct
    {
        // define fields and properties for the shared struct here
        public int test1;
        public int test2;
        public Gcp_Info GcpInfo;
        public Srm_Info[] SrmInfo;
        public Dio_Packet[] DioPacket;
        public Srm_Packet[] SrmPacket;
        public Srm_State[] SrmState;
        public int SharedField { get; set; }
    }
    public class singletonClass
    {
        private static readonly Lazy<singletonClass> lazyInstance = new Lazy<singletonClass>(() => new singletonClass());
        public static singletonClass Instance { get { return lazyInstance.Value; } }
        public int test;
        private SharedStruct SharedData;

        private singletonClass()
        {
            Console.WriteLine("Create Called Singletone class");

            // 구조체 배열 동적할당
            Srm_Info[] srmInfo = new Srm_Info[3];
            Dio_Packet[] dioPacket = new Dio_Packet[3];
            Srm_Packet[] srmPacket = new Srm_Packet[3];
            dioPacket[0].DI = new ushort[cConstDefine.IOCOUNT];
            dioPacket[0].DO = new ushort[cConstDefine.IOCOUNT];
            dioPacket[1].DI = new ushort[cConstDefine.IOCOUNT];
            dioPacket[1].DO = new ushort[cConstDefine.IOCOUNT];
            dioPacket[2].DI = new ushort[cConstDefine.IOCOUNT];
            dioPacket[2].DO = new ushort[cConstDefine.IOCOUNT];
            Srm_State[] srmState = new Srm_State[3];

            SharedData = new SharedStruct();
            SharedData.SrmInfo = srmInfo;
            SharedData.DioPacket = dioPacket;
            SharedData.SrmPacket = srmPacket;
            SharedData.SrmState = srmState;
        }

        public ref SharedStruct str { get { return ref SharedData; } }


        public int srmNum { get; set; }

    }
}
