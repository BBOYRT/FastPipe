# 📦 PIPE_LIB DLL 函數說明
AI生成說明文黨(有問題詢問比較快)
## 🧠 命名建議（DLL名稱）

建議 DLL / Namespace：

```csharp
PIPE_LIB
```

---

# 🧩 Client 類別（Pipe Client）

## 📌 功能

提供 NamedPipe Client 端連線、收發訊息與自動重連機制。

---

## 🔧 方法說明

---

## 1️⃣ init_Client

```csharp
public string init_Client(string pipeName)
```

### 📌 功能

初始化 Client，設定 Pipe 名稱。

### 📥 參數

* `pipeName`：Pipe server 名稱

### 📤 回傳

* `"ok"`：初始化成功
* `"input is null"`：參數為空
* `"please first deinit"`：尚未釋放舊連線

### ⚠️ 注意

必須先呼叫此方法才能 Start Client

---

## 2️⃣ Begin_Client

```csharp
public string Begin_Client()
```

### 📌 功能

啟動 Client 背景執行緒：

* 自動連線
* 收資料
* 發資料

### 📤 回傳

* `"ok"`：啟動成功
* `"init_server not run"`：尚未 init

---

## 3️⃣ send_data

```csharp
public void send_data(string DATA)
```

### 📌 功能

將資料加入發送 Queue（非同步送出）

### 📥 參數

* `DATA`：要送出的字串

### ⚠️ 特性

* 不阻塞
* 由背景 thread 發送

---

## 4️⃣ read_data

```csharp
public string read_data()
```

### 📌 功能

讀取接收到的資料（FIFO）

### 📤 回傳

* 有資料：回傳字串
* 無資料：回傳 `""`

---

## 5️⃣ ckick_Connect

```csharp
public string ckick_Connect()
```

### 📌 功能

檢查 Client 是否連線

### 📤 回傳

* `"True"` / `"False"`
* `"init_server not run"`：尚未初始化

---

# 🧩 Server 類別（Pipe Server）

---

## 📌 功能

提供 NamedPipe Server：

* 接收 Client 連線
* 收發訊息
* 自動重建 pipe

---

## 🔧 方法說明

---

## 1️⃣ init_server

```csharp
public string init_server(string pipe_name)
```

### 📌 功能

初始化 Server pipe 名稱

### 📥 參數

* `pipe_name`：Pipe 名稱

### 📤 回傳

* `"ok"`：成功
* `"input is null"`：參數錯誤
* `"please first deinit"`：已初始化

---

## 2️⃣ Begin_server

```csharp
public string Begin_server()
```

### 📌 功能

啟動 Server 背景服務：

* 等待連線
* 收資料
* 發資料

### 📤 回傳

* `"ok"`
* `"init_server not run"`

---

## 3️⃣ send_data

```csharp
public void send_data(string DATA)
```

### 📌 功能

將資料加入發送佇列

---

## 4️⃣ read_data

```csharp
public string read_data()
```

### 📌 功能

讀取 Client 傳來的資料

### 📤 回傳

* 有資料：string
* 無資料：""（空字串）

---

## 5️⃣ ckick_Connect

```csharp
public string ckick_Connect()
```

### 📌 功能

檢查 Server 是否已連線

### 📤 回傳

* `"True"` / `"False"`
* `"init_server not run"`

---

# ⚙️ 系統設計特性

## 🔄 1. 非同步架構

* Task.Run 背景執行
* 不阻塞主執行緒

---

## 📦 2. Queue 緩衝機制

* send_queue → 發送 buffer
* read_queue → 接收 buffer

---

## 🔌 3. 自動重連

* Client：斷線自動 reconnect
* Server：重新建立 NamedPipeServerStream

---
