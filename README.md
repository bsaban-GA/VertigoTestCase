# Vertigo Games – Wheel of Fortune Test Case

Oyuncunun 60 bölge (zone) boyunca 8 dilimli bir çarkı çevirdiği, tamamen UI tabanlı bir Unity oyunu.

- **Unity:** 2021.3.45f2 · **Platform:** Android (yatay ekran)
- **Kullanılanlar:** TextMeshPro, DOTween, ScriptableObject, Sprite Atlas
- **APK:** [GitHub Releases](https://github.com/bsaban-GA/VertigoTestCase/releases) sayfasından indirilebilir.

## Oyun Kuralları

| Bölge | Ne zaman | Çark | İçerik |
|---|---|---|---|
| Normal | Diğer tüm bölgeler | Bronz | 7 ödül + 1 bomba |
| Güvenli (Safe) | 1. bölge ve her 5. bölge | Gümüş | 8 ödül, bomba yok |
| Süper (Super) | Her 30. bölge | Altın | 8 özel ödül, bomba yok |

- 30 ve 60 hem 5'in hem 30'un katıdır; bu bölgelerde **Süper** kuralı geçerlidir.
- Oyun ekrana dokunarak başlar. Çevirme tuşuna basıldığı anda sonuç belirlenir; çark animasyonu bu sonuca gider. Çark dönerken ekrana dokunmak animasyonu atlar, sonuç değişmez.
- Ödül gelirse toplanan ödüllere eklenir ve bir sonraki bölgeye geçilir.
- Bomba gelirse toplanan ödüller gizlenir ve **"Pay & Continue / Leave Game"** penceresi açılır:
  - **Pay & Continue:** Altın ödenir (yeterli altın yoksa buton pasiftir). Toplanan ödüller korunur ve **aynı bölge** tekrar çevrilir. Bombanın yuvası boşalır; çark o yuvaya asla gelmez.
  - **Leave Game:** Bu turda toplanan her şey kaybedilir.
- Oyuncu yalnızca güvenli/süper bölgelerde ve **çevirmeden önce** ödülleriyle oyundan çıkabilir.
- Oyun şu durumlarda biter: 60 bölgenin tamamı geçilir, oyuncu güvenli/süper bölgede çıkar ya da bombadan sonra pes eder.

## Teknik Yapı

### Katmanlar

```
WheelGameInstaller (MonoBehaviour, tek kurulum noktası)
  └─ tüm nesneleri oluşturur ve bağımlılıklarını verir
        │
WheelGamePresenter (saf C#)
  ├─ oturum olaylarını dinler  → view'ları günceller
  └─ view olaylarını dinler    → oturum metodlarını çağırır
        │                               │
Views (MonoBehaviour)          WheelGameSession (Core, saf C#)
gösterir, canlandırır,         oyunun kuralları; arayüzden habersiz
tıklamaları olay olarak bildirir
```

Yapı **Model-View-Presenter** desenini izler. Kurallar arayüzü bilmez; arayüz kuralları bilmez; ikisini yalnızca presenter bağlar.

### Assembly'ler

| Assembly | İçerik | Bağımlılık |
|---|---|---|
| `Vertigo.TestCase.Core` | Kurallar, veri (ScriptableObject), ekonomi. MonoBehaviour yok. | — |
| `Vertigo.TestCase.UI` | View'lar, presenter, installer, DOTween animasyonları | Core, UGUI, TextMeshPro, DOTween |
| `Vertigo.TestCase.Tests` | EditMode testleri (yalnızca editörde) | Core |

Bağımlılık tek yönlüdür (UI → Core). Core, arayüze erişemez; bu ayrımı derleyici zorlar.

### Önemli Sınıflar

**Core (`Assets/Main/Scripts/Core`)**
- `ZoneRules`: Bir bölgenin tipini (normal / güvenli / süper) belirler.
- `WheelGenerator`: Çarkın 8 yuvasını ağırlıklı rastgele seçimle doldurur. Aynı ödül bir çarkta iki kez çıkmaz; miktarlar bölge ilerledikçe artar.
- `Wheel`, `WheelSlot`: Değiştirilemez (immutable) çark verisi. Yuva tipleri: ödül, bomba, boş. Diriliş sonrası bomba yuvası boşalır ve `LandableSlotIndices` listesinden çıkar.
- `WheelGameSession`: Durum makinesi (`NotStarted → AwaitingSpin → Spinning → AwaitingRevive → Ended`). `Spin()` sonucu hemen belirler, `ResolveSpin()` animasyon bitince (ya da atlanınca) sonucu uygular.
- `RunRewards`: Bu turda toplanan, bomba ile kaybedilebilecek ödüller.
- `PlayerInventory`: Kalıcı ödüller ve altın (`ICurrencyWallet`, `IRewardBank`). Kayıt sistemi yoktur; uygulama kapanınca başlangıç altınına döner.
- `IRandomProvider`: Rastgelelik arayüzü. Testler sonucu (bomba / ödül) kesin olarak seçebilsin diye enjekte edilir.

**UI (`Assets/Main/Scripts/UI`)**
- `WheelView`, `WheelSlotView`: Çark, yuvalar ve DOTween ile dönüş. Atlama `tween.Complete()` ile yapılır; böylece atlanan dönüş, normal dönüşle aynı kod yolundan geçer.
- `ZoneBarView`: 60 bölgelik şerit; geçerli bölge sabit bir işaretin altına kayar.
- `CollectedRewardsView`: Toplanan ödüller listesi; bombada gizlenir, dirilişte geri gelir.
- `BombPopupView`, `ResultPopupView` (`PopupView` tabanından): Açılış/kapanış animasyonu ortak sınıftadır.
- `RewardFlyView`: Kazanılan ödül simgesi çarktan listeye uçar.
- `WheelGamePresenter`, `WheelGameInstaller`: Akış ve kurulum.

### SOLID

| İlke | Projede nerede |
|---|---|
| Single Responsibility | Her sınıfın tek işi var: bölge tipi, çark içeriği, tur ödülleri, bakiye, oyun akışı, her view kendi alanı. |
| Open/Closed | Yeni ödül veya çark tipi yeni bir asset'tir, kod değişmez. Yeni popup, `PopupView`'dan türetilir. |
| Liskov Substitution | Her `IRandomProvider` uygulaması (sistem, sabit seed, test sahtesi) aynı şekilde çalışır. |
| Interface Segregation | Oturum yalnızca ihtiyacı olanı görür: ödeme için `ICurrencyWallet`, ödül yatırmak için `IRewardBank`. |
| Dependency Inversion | Bağımlılıklar constructor ile verilir; singleton ve `FindObjectOfType` yoktur. |

### Brifteki UI Kuralları

| Kural | Uygulama |
|---|---|
| Canvas Expand | Canvas Scaler: Scale With Screen Size, 1920×1080, **Expand**. 20:9, 16:9 ve 4:3'te test edildi. |
| TextMeshPro | Tüm metinler `TMP_Text`. |
| `_value` son eki | Çalışma anında içeriği değişen her metin/görsel `_value` ile biter. |
| Genelden özele isimlendirme | `ui_<tip>_<alan>_<detay>`, ör. `ui_image_wheel_base_value`. |
| Gereksiz Raycast Target / Maskable yok | Raycast yalnızca butonlarda ve popup arka planlarında; Maskable yalnızca `RectMask2D` altında. |
| Animasyon kök objede değil | Tüm tween'ler alt objeleri hedefler (`*_animated`, `ui_rect_wheel_rotator`). |
| Doğru anchor/pivot | Her panel ait olduğu kenara anchor'lıdır; `SafeAreaFitter` çentikli ekranları karşılar. |
| Buton referansları `OnValidate` ile | `UIBinding.BindChild` butonu isminden bulur ve referansı kaydeder. |
| Editörden OnClick yok | Tüm dinleyiciler kodda `AddListener` / `RemoveListener` çiftleriyle eklenir. |
| Sliced sprite, esneme yok | Çerçeve ve paneller 9-slice (Sliced); ikonlar Simple + Preserve Aspect. |
| Sprite Atlas | 3 atlas: `atlas_ui_wheel`, `atlas_ui_icons`, `atlas_ui_common` (Android'de ASTC). |

### Testler

**Window → General → Test Runner → EditMode → Run All**: 40 test. Bölge tipleri, çark üretimi, bomba, diriliş, pes etme, çıkış, 60 bölgenin tamamı ve ScriptableObject'lerin Unity serileştirmesinden doğru geçtiği test edilir.

### Klasörler

```
Assets/Main/
  Art/Sprites, Art/Atlases   Görseller ve sprite atlasları
  Data/Rewards               Ödül tanımları (RewardItem)
  Data/Wheels                Çark tanımları (WheelDefinition)
  Data/Config                Oyun ayarları (GameConfig)
  Prefabs/UI                 Yuva, bölge hücresi, ödül satırı
  Scenes/Gameplay.unity      Tek oyun sahnesi
  Scripts/Core, UI, Tests
```

## Verileri Değiştirme

Oyunun tüm dengesi ScriptableObject asset'lerindedir. **Hiçbir değişiklik kod gerektirmez**; Inspector'dan düzenlenir ve bir sonraki Play'de geçerli olur.

### Oyun Ayarları: `Data/Config/game_config`

| Alan | Etkisi | Varsayılan |
|---|---|---|
| Zone Rules → Total Zones | Toplam bölge sayısı | 60 |
| Zone Rules → Safe Zone Interval | Kaç bölgede bir güvenli bölge | 5 |
| Zone Rules → Super Zone Interval | Kaç bölgede bir süper bölge | 30 |
| Normal / Safe / Super Wheel | Her bölge tipinde kullanılacak çark | bronz / gümüş / altın |
| Reward Growth Per Zone | Bölge başına miktar artışı (0.1 = bölge başına %10) | 0.1 |
| Revive Rules → Base Cost | İlk dirilişin altın bedeli | 25 |
| Revive Rules → Cost Multiplier | Her dirilişte bedelin çarpanı | 2 (25 → 50 → 100) |
| Revive Rules → Max Cost | Diriliş bedelinin üst sınırı | 1000 |
| Gold Coin | Diriliş için harcanan ödül | `reward_gold` |
| Starting Gold | Oyun başlarken sahip olunan altın | 100 |

1. bölgenin her zaman güvenli olması sabit bir kuraldır, ayarlardan değiştirilmez.

### Çarklar: `Data/Wheels/wheel_bronze`, `wheel_silver`, `wheel_golden`

| Alan | Etkisi |
|---|---|
| Base Sprite / Indicator Sprite | Çarkın ve okun görseli |
| Title / Subtitle | Çarkın üstünde ve altında yazan metin |
| Contains Bomb | Çarkta bomba olup olmadığı (normal çarkta açık, diğerlerinde kapalı olmalı) |
| Reward Pool | Bu çarkta çıkabilecek ödüller (aşağıda) |

**Reward Pool satırları:**

| Alan | Etkisi |
|---|---|
| Item | Ödül (`Data/Rewards` içinden) |
| Weight | Çarkta görünme sıklığı. Göreli bir değerdir: 30, 10'dan üç kat daha sık çıkar. Toplamın 100 olması gerekmez. |
| Min Amount / Max Amount | Bu aralıktan rastgele bir miktar seçilir. |
| Scale With Zone | Açıksa miktar bölge ilerledikçe artar. Silah, sandık gibi tekil ödüllerde kapalı tutulmalıdır. |

- Ağırlık, bir ödülün **çarkta yer alma** olasılığını belirler. Çarkın **nereye duracağı** her zaman eşit olasılıklıdır (yuva başına 1/8).
- Bombalı bir çark en az **7**, bombasız bir çark en az **8 farklı** ödül içermelidir. Yetersizse Console'da uyarı çıkar.

### Yeni Ödül Ekleme

1. `Data/Rewards` içinde sağ tık → **Create → Config → RewardItem**.
2. Dosyaya `reward_<isim>` adını verin, **Display Name** ve **Icon** alanlarını doldurun.
3. Görsel `Art/Sprites/Icons` klasörüne konduysa `atlas_ui_icons` atlasına otomatik girer.
4. Ödülü bir veya birden fazla çarkın **Reward Pool** listesine ekleyin.

### Yeni Bölge / Çark Tipi

Mevcut üç tipin görünümü ve içeriği yalnızca asset'lerden değiştirilir. Tamamen yeni bir bölge tipi (ör. "her 10. bölge 2 kat ödül") eklemek kod gerektirir: `ZoneType`'a yeni değer, `ZoneRules.GetZoneType`'a kural, `GameConfig.GetWheel`'e yeni çark alanı ve yeni bir `WheelDefinition` asset'i.

### Sahnedeki Ayarlar

Sahnedeki bileşenlerde ince ayar yapılabilir:

| Bileşen | Alanlar |
|---|---|
| `WheelGameInstaller` | **Fixed Seed**: 0 her açılışta farklı oyun üretir; başka bir sayı aynı çarkları ve sonuçları tekrar oynatır (hata ayıklamak için). Kullanılan seed Console'a yazılır. |
| `WheelView` | Spin Duration, Full Turns, Landing Jitter, Bomb Shake Duration/Strength |
| `ZoneBarView` | Bölge hücresi görselleri, aralık, kayma süresi |
| `RewardFlyView` / `CollectedRewardsView` | Uçuş süresi ve satırın vurgulanma gecikmesi (birbirine eşit tutulmalı) |
| `BombPopupView` | Open Delay (çark sarsıntısıyla eşit tutulmalı) |
