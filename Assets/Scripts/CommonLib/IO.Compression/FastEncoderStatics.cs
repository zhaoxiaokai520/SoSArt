using System;
using System.Diagnostics;

namespace Unity.IO.Compression
{
	internal static class FastEncoderStatics
	{
		internal static readonly byte[] FastEncoderTreeStructureData;

		internal static readonly byte[] BFinalFastEncoderTreeStructureData;

		internal static readonly uint[] FastEncoderLiteralCodeInfo;

		internal static readonly uint[] FastEncoderDistanceCodeInfo;

		internal static readonly uint[] BitMask;

		internal static readonly byte[] ExtraLengthBits;

		internal static readonly byte[] ExtraDistanceBits;

		internal const int NumChars = 256;

		internal const int NumLengthBaseCodes = 29;

		internal const int NumDistBaseCodes = 30;

		internal const uint FastEncoderPostTreeBitBuf = 34u;

		internal const int FastEncoderPostTreeBitCount = 9;

		internal const uint NoCompressionHeader = 0u;

		internal const int NoCompressionHeaderBitCount = 3;

		internal const uint BFinalNoCompressionHeader = 1u;

		internal const int BFinalNoCompressionHeaderBitCount = 3;

		internal const int MaxCodeLen = 16;

		private static byte[] distLookup;

		static FastEncoderStatics()
		{
			FastEncoderStatics.FastEncoderTreeStructureData = new byte[]
			{
				236,
				189,
				7,
				96,
				28,
				73,
				150,
				37,
				38,
				47,
				109,
				202,
				123,
				127,
				74,
				245,
				74,
				215,
				224,
				116,
				161,
				8,
				128,
				96,
				19,
				36,
				216,
				144,
				64,
				16,
				236,
				193,
				136,
				205,
				230,
				146,
				236,
				29,
				105,
				71,
				35,
				41,
				171,
				42,
				129,
				202,
				101,
				86,
				101,
				93,
				102,
				22,
				64,
				204,
				237,
				157,
				188,
				247,
				222,
				123,
				239,
				189,
				247,
				222,
				123,
				239,
				189,
				247,
				186,
				59,
				157,
				78,
				39,
				247,
				223,
				255,
				63,
				92,
				102,
				100,
				1,
				108,
				246,
				206,
				74,
				218,
				201,
				158,
				33,
				128,
				170,
				200,
				31,
				63,
				126,
				124,
				31,
				63
			};
			FastEncoderStatics.BFinalFastEncoderTreeStructureData = new byte[]
			{
				237,
				189,
				7,
				96,
				28,
				73,
				150,
				37,
				38,
				47,
				109,
				202,
				123,
				127,
				74,
				245,
				74,
				215,
				224,
				116,
				161,
				8,
				128,
				96,
				19,
				36,
				216,
				144,
				64,
				16,
				236,
				193,
				136,
				205,
				230,
				146,
				236,
				29,
				105,
				71,
				35,
				41,
				171,
				42,
				129,
				202,
				101,
				86,
				101,
				93,
				102,
				22,
				64,
				204,
				237,
				157,
				188,
				247,
				222,
				123,
				239,
				189,
				247,
				222,
				123,
				239,
				189,
				247,
				186,
				59,
				157,
				78,
				39,
				247,
				223,
				255,
				63,
				92,
				102,
				100,
				1,
				108,
				246,
				206,
				74,
				218,
				201,
				158,
				33,
				128,
				170,
				200,
				31,
				63,
				126,
				124,
				31,
				63
			};
			FastEncoderStatics.FastEncoderLiteralCodeInfo = new uint[]
			{
				55278u,
				317422u,
				186350u,
				448494u,
				120814u,
				382958u,
				251886u,
				514030u,
				14318u,
				51180u,
				294u,
				276462u,
				145390u,
				407534u,
				79854u,
				341998u,
				210926u,
				473070u,
				47086u,
				309230u,
				178158u,
				440302u,
				112622u,
				374766u,
				243694u,
				505838u,
				30702u,
				292846u,
				161774u,
				423918u,
				6125u,
				96238u,
				1318u,
				358382u,
				9194u,
				116716u,
				227310u,
				489454u,
				137197u,
				25578u,
				2920u,
				3817u,
				23531u,
				5098u,
				1127u,
				7016u,
				3175u,
				12009u,
				1896u,
				5992u,
				3944u,
				7913u,
				8040u,
				16105u,
				21482u,
				489u,
				232u,
				8681u,
				4585u,
				4328u,
				12777u,
				13290u,
				2280u,
				63470u,
				325614u,
				6376u,
				2537u,
				1256u,
				10729u,
				5352u,
				6633u,
				29674u,
				56299u,
				3304u,
				15339u,
				194542u,
				14825u,
				3050u,
				1513u,
				19434u,
				9705u,
				10220u,
				5609u,
				13801u,
				3561u,
				11242u,
				75756u,
				48107u,
				456686u,
				129006u,
				42988u,
				31723u,
				391150u,
				64491u,
				260078u,
				522222u,
				4078u,
				806u,
				615u,
				2663u,
				1639u,
				1830u,
				7400u,
				744u,
				3687u,
				166u,
				108524u,
				11753u,
				1190u,
				359u,
				2407u,
				678u,
				1383u,
				71661u,
				1702u,
				422u,
				1446u,
				3431u,
				4840u,
				2792u,
				7657u,
				6888u,
				2027u,
				202733u,
				26604u,
				38893u,
				169965u,
				266222u,
				135150u,
				397294u,
				69614u,
				331758u,
				200686u,
				462830u,
				36846u,
				298990u,
				167918u,
				430062u,
				102382u,
				364526u,
				233454u,
				495598u,
				20462u,
				282606u,
				151534u,
				413678u,
				85998u,
				348142u,
				217070u,
				479214u,
				53230u,
				315374u,
				184302u,
				446446u,
				118766u,
				380910u,
				249838u,
				511982u,
				12270u,
				274414u,
				143342u,
				405486u,
				77806u,
				339950u,
				208878u,
				471022u,
				45038u,
				307182u,
				176110u,
				438254u,
				110574u,
				372718u,
				241646u,
				503790u,
				28654u,
				290798u,
				159726u,
				421870u,
				94190u,
				356334u,
				225262u,
				487406u,
				61422u,
				323566u,
				192494u,
				454638u,
				126958u,
				389102u,
				258030u,
				520174u,
				8174u,
				270318u,
				139246u,
				401390u,
				73710u,
				335854u,
				204782u,
				466926u,
				40942u,
				303086u,
				172014u,
				434158u,
				106478u,
				368622u,
				237550u,
				499694u,
				24558u,
				286702u,
				155630u,
				417774u,
				90094u,
				352238u,
				221166u,
				483310u,
				57326u,
				319470u,
				188398u,
				450542u,
				122862u,
				385006u,
				253934u,
				516078u,
				16366u,
				278510u,
				147438u,
				409582u,
				81902u,
				344046u,
				212974u,
				475118u,
				49134u,
				311278u,
				180206u,
				442350u,
				114670u,
				376814u,
				245742u,
				507886u,
				32750u,
				294894u,
				163822u,
				425966u,
				98286u,
				104429u,
				235501u,
				22509u,
				360430u,
				153581u,
				229358u,
				88045u,
				491502u,
				219117u,
				65518u,
				327662u,
				196590u,
				458734u,
				131054u,
				132u,
				3u,
				388u,
				68u,
				324u,
				197u,
				709u,
				453u,
				966u,
				1990u,
				38u,
				1062u,
				935u,
				2983u,
				1959u,
				4007u,
				551u,
				1575u,
				2599u,
				3623u,
				104u,
				2152u,
				4200u,
				6248u,
				873u,
				4969u,
				9065u,
				13161u,
				1770u,
				9962u,
				18154u,
				26346u,
				5867u,
				14059u,
				22251u,
				30443u,
				38635u,
				46827u,
				55019u,
				63211u,
				15852u,
				32236u,
				48620u,
				65004u,
				81388u,
				97772u,
				114156u,
				130540u,
				27629u,
				60397u,
				93165u,
				125933u,
				158701u,
				191469u,
				224237u,
				257005u,
				1004u,
				17388u,
				33772u,
				50156u,
				66540u,
				82924u,
				99308u,
				115692u,
				7150u,
				39918u,
				72686u,
				105454u,
				138222u,
				170990u,
				203758u,
				236526u,
				269294u,
				302062u,
				334830u,
				367598u,
				400366u,
				433134u,
				465902u,
				498670u,
				92144u,
				223216u,
				354288u,
				485360u,
				616432u,
				747504u,
				878576u,
				1009648u,
				1140720u,
				1271792u,
				1402864u,
				1533936u,
				1665008u,
				1796080u,
				1927152u,
				2058224u,
				34799u,
				100335u,
				165871u,
				231407u,
				296943u,
				362479u,
				428015u,
				493551u,
				559087u,
				624623u,
				690159u,
				755695u,
				821231u,
				886767u,
				952303u,
				1017839u,
				59376u,
				190448u,
				321520u,
				452592u,
				583664u,
				714736u,
				845808u,
				976880u,
				1107952u,
				1239024u,
				1370096u,
				1501168u,
				1632240u,
				1763312u,
				1894384u,
				2025456u,
				393203u,
				917491u,
				1441779u,
				1966067u,
				2490355u,
				3014643u,
				3538931u,
				4063219u,
				4587507u,
				5111795u,
				5636083u,
				6160371u,
				6684659u,
				7208947u,
				7733235u,
				8257523u,
				8781811u,
				9306099u,
				9830387u,
				10354675u,
				10878963u,
				11403251u,
				11927539u,
				12451827u,
				12976115u,
				13500403u,
				14024691u,
				14548979u,
				15073267u,
				15597555u,
				16121843u,
				16646131u,
				262131u,
				786419u,
				1310707u,
				1834995u,
				2359283u,
				2883571u,
				3407859u,
				3932147u,
				4456435u,
				4980723u,
				5505011u,
				6029299u,
				6553587u,
				7077875u,
				7602163u,
				8126451u,
				8650739u,
				9175027u,
				9699315u,
				10223603u,
				10747891u,
				11272179u,
				11796467u,
				12320755u,
				12845043u,
				13369331u,
				13893619u,
				14417907u,
				14942195u,
				15466483u,
				15990771u,
				16515059u,
				524275u,
				1048563u,
				1572851u,
				2097139u,
				2621427u,
				3145715u,
				3670003u,
				4194291u,
				4718579u,
				5242867u,
				5767155u,
				6291443u,
				6815731u,
				7340019u,
				7864307u,
				8388595u,
				8912883u,
				9437171u,
				9961459u,
				10485747u,
				11010035u,
				11534323u,
				12058611u,
				12582899u,
				13107187u,
				13631475u,
				14155763u,
				14680051u,
				15204339u,
				15728627u,
				16252915u,
				16777203u,
				124913u,
				255985u,
				387057u,
				518129u,
				649201u,
				780273u,
				911345u,
				1042417u,
				1173489u,
				1304561u,
				1435633u,
				1566705u,
				1697777u,
				1828849u,
				1959921u,
				2090993u,
				2222065u,
				2353137u,
				2484209u,
				2615281u,
				2746353u,
				2877425u,
				3008497u,
				3139569u,
				3270641u,
				3401713u,
				3532785u,
				3663857u,
				3794929u,
				3926001u,
				4057073u,
				18411u
			};
			FastEncoderStatics.FastEncoderDistanceCodeInfo = new uint[]
			{
				3846u,
				130826u,
				261899u,
				524043u,
				65305u,
				16152u,
				48936u,
				32552u,
				7991u,
				24375u,
				3397u,
				12102u,
				84u,
				7509u,
				2148u,
				869u,
				1140u,
				4981u,
				3204u,
				644u,
				2708u,
				1684u,
				3748u,
				420u,
				2484u,
				2997u,
				1476u,
				7109u,
				2005u,
				6101u,
				0u,
				256u
			};
			FastEncoderStatics.BitMask = new uint[]
			{
				0u,
				1u,
				3u,
				7u,
				15u,
				31u,
				63u,
				127u,
				255u,
				511u,
				1023u,
				2047u,
				4095u,
				8191u,
				16383u,
				32767u
			};
			FastEncoderStatics.ExtraLengthBits = new byte[]
			{
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				1,
				1,
				1,
				2,
				2,
				2,
				2,
				3,
				3,
				3,
				3,
				4,
				4,
				4,
				4,
				5,
				5,
				5,
				5,
				0
			};
			FastEncoderStatics.ExtraDistanceBits = new byte[]
			{
				0,
				0,
				0,
				0,
				1,
				1,
				2,
				2,
				3,
				3,
				4,
				4,
				5,
				5,
				6,
				6,
				7,
				7,
				8,
				8,
				9,
				9,
				10,
				10,
				11,
				11,
				12,
				12,
				13,
				13,
				0,
				0
			};
			FastEncoderStatics.distLookup = new byte[512];
			int num = 0;
			int i;
			for (i = 0; i < 16; i++)
			{
				for (int j = 0; j < 1 << (int)FastEncoderStatics.ExtraDistanceBits[i]; j++)
				{
					FastEncoderStatics.distLookup[num++] = (byte)i;
				}
			}
			num >>= 7;
			while (i < 30)
			{
				for (int k = 0; k < 1 << (int)(FastEncoderStatics.ExtraDistanceBits[i] - 7); k++)
				{
					FastEncoderStatics.distLookup[256 + num++] = (byte)i;
				}
				i++;
			}
		}

		internal static int GetSlot(int pos)
		{
			return (int)FastEncoderStatics.distLookup[(pos < 256) ? pos : (256 + (pos >> 7))];
		}

		public static uint BitReverse(uint code, int length)
		{
			uint num = 0u;
			Debug.Assert(length > 0 && length <= 16, "Invalid len");
			do
			{
				num |= (code & 1u);
				num <<= 1;
				code >>= 1;
			}
			while (--length > 0);
			return num >> 1;
		}
	}
}
